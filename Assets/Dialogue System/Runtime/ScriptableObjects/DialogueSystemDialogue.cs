using DialogueSystem.Runtime.Data;
using DialogueSystem.Runtime.Enumerations;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Runtime.ScriptableObjects
{
    public class DialogueSystemDialogue : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField] [field: TextArea] public string Text { get; set; }

        [field: SerializeField] public List<DialogueSystemDialogueChoiceData> Choices { get; private set; }

        [field: SerializeField] public DialogueType Type { get; set; }

        [field: SerializeField] public bool IsStartingDialogue { get; private set; }

        public void Initialize(string dialogueName, string text, List<DialogueSystemDialogueChoiceData> choices, DialogueType dialogueType, bool isStartingDialogue)
        {
            Name = dialogueName;
            Text = text;
            Choices = choices;
            Type = dialogueType;
            IsStartingDialogue = isStartingDialogue;
        }
    }
}