using DialogueSystem.Runtime.ScriptableObjects;
using UnityEngine;

namespace DialogueSystem.Runtime.Scripts
{
    public class Dialogue : MonoBehaviour
    {
        [SerializeField] private DialogueSystemDialogueContainer dialogueContainer;
        [SerializeField] private DialogueSystemDialogueGroup dialogueGroup;
        [SerializeField] private DialogueSystemDialogue dialogue;
        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialoguesOnly;
        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;
    }
}