using System.Collections;
using System.Collections.Generic;
using System.Text;
using DialogueSystem.Runtime.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    [SerializeField] private float timeSpan = 0.1f;
    [SerializeField] private GameObject dialogueObject;
    [SerializeField] private GameObject buttonPrefab;
    
    private Coroutine textCoroutine;
    
    private void Awake()
    {
        Debug.AssertFormat(dialogueObject, "{0} Prefab {1} is not set!", typeof(GameObject), dialogueObject);
        Debug.AssertFormat(buttonPrefab, "{0} Prefab {1} is not set!", typeof(GameObject), buttonPrefab);
    }

    private void Start()
    {
        StartDialogue();
    }
    
    private void OnDestroy()
    {
        if (dialogueObject.TryGetComponent(out Dialogue dialogue))
        {
            dialogue.Reset();
        }
    }

    public void StartDialogue()
    {
        var buttons = new List<GameObject>();
        var dialogue = dialogueObject.GetComponent<Dialogue>();
        GenerateChoiceButtons(buttons, dialogue, buttonPrefab);
    }
    
    private void GenerateChoiceButtons(List<GameObject> buttons, Dialogue dialogue, GameObject buttonPref)
    {
        if (buttons == null || !dialogue || !buttonPref)
        {
            return;
        }
        
        foreach (var button in buttons)
        {
            Destroy(button);
        }
        
        buttons.Clear();
        var firstChild = transform.GetChild(0);
        if (firstChild && firstChild.TryGetComponent(out Text dialogueText) && gameObject.activeInHierarchy)
        {
            textCoroutine = StartCoroutine(SetText(dialogueText, dialogue.Text, timeSpan, dialogue.GroupName + ": "));
        }
        
        for (var i = 0; i < dialogue.ChoiceCount; i++)
        {
            var index = i;
            var buttonObject = Instantiate(buttonPref, transform);
            if (buttonObject.TryGetComponent(out Button button))
            {
                button.onClick.AddListener(() =>
                {
                    if (textCoroutine != null)
                    {
                        StopCoroutine(textCoroutine);
                        textCoroutine = null;
                        firstChild.GetComponent<Text>().text = $"{dialogue.GroupName}: {dialogue.Text}";
                        return;
                    }
                    
                    dialogue.Choose(index);
                    if (dialogue.ChoiceCount == 0)
                    {
                        dialogue.Reset();
                        Destroy(gameObject);
                        return;
                    }
                    
                    GenerateChoiceButtons(buttons, dialogue, buttonPref);
                });
                var buttonText = buttonObject.GetComponentInChildren<Text>();
                if (buttonText)
                {
                    var choiceText = dialogue.ChoiceText(index);
                    buttonText.text = choiceText.Equals("Next Dialogue") ? "继续" : choiceText;
                }
            }
            
            buttons.Add(buttonObject);
        }
    }

    private IEnumerator SetText(Text text, string content, float time, string header = null)
    {
        if (!text || content == null || time < 0)
        {
            yield break;
        }
        
        var charArray = content.ToCharArray();
        var stringBuilder = new StringBuilder();
        stringBuilder = stringBuilder.Append(header);
        var waitForSeconds = new WaitForSeconds(time);
        foreach (var c in charArray)
        {
            stringBuilder = stringBuilder.Append(c);
            text.text = stringBuilder.ToString();
            yield return waitForSeconds;
        }

        textCoroutine = null;
    }
}
