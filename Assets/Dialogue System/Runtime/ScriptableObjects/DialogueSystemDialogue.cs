using DialogueSystem.Runtime.Data;
using DialogueSystem.Runtime.Enumerations;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Runtime.ScriptableObjects
{
    public class DialogueSystemDialogue : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }

        [field: SerializeField] [field: TextArea] public string Text { get; set; }

        [field: SerializeField] public List<DialogueSystemDialogueChoiceData> Choices { get; set; }

        [field: SerializeField] public DialogueType DialogueType { get; set; }

        [field: SerializeField] public bool IsStartingDialogue { get; set; }

        public void Initialize(string dialogueName, string text, List<DialogueSystemDialogueChoiceData> choices, DialogueType dialogueType, bool isStartingDialogue)
        {
            DialogueName = dialogueName;
            Text = text;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
        }
    }
}