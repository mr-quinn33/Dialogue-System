using System.Collections.Generic;
using DialogueSystem.Runtime.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogue;
    [SerializeField] private GameObject buttonPrefab;
    
    private void Awake()
    {
        Debug.AssertFormat(dialogue, "{0} Prefab {1} is not set!", typeof(GameObject), dialogue);
        Debug.AssertFormat(buttonPrefab, "{0} Prefab {1} is not set!", typeof(GameObject), buttonPrefab);
    }

    public void StartDialogue()
    {
        var buttons = new List<GameObject>();
        _ = GenerateChoiceButtons(buttons, transform, dialogue.GetComponent<Dialogue>(), buttonPrefab);
    }
    
    private static List<GameObject> GenerateChoiceButtons(List<GameObject> buttons, Transform parent, Dialogue dialogue,
        GameObject buttonPrefab)
    {
        if (!dialogue)
        {
            return null;
        }
        
        var firstChild = parent.GetChild(0);
        if (firstChild && firstChild.TryGetComponent(out TMP_Text dialogueText))
        {
            dialogueText.SetText(dialogue.Text);
        }
        
        foreach (var button in buttons)
        {
            Destroy(button);
        }
        
        buttons.Clear();
        var choiceButtons = new List<GameObject>();
        for (var i = 0; i < dialogue.ChoiceCount; i++)
        {
            var index = i;
            var buttonObject = Instantiate(buttonPrefab, parent);
            if (buttonObject.TryGetComponent(out Button button))
            {
                button.onClick.AddListener(() =>
                {
                    dialogue.Choose(index);
                    choiceButtons = GenerateChoiceButtons(choiceButtons, parent, dialogue, buttonPrefab);
                });
                var buttonText = buttonObject.GetComponentInChildren<TMP_Text>();
                if (buttonText)
                {
                    buttonText.SetText(dialogue.ChoiceText(index));
                }
            }

            choiceButtons.Add(buttonObject);
        }

        return choiceButtons;
    }
}
