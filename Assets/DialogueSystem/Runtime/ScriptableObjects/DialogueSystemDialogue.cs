using System.Collections.Generic;
using DialogueSystem.Runtime.Data;
using DialogueSystem.Runtime.Enumerations;
using UnityEngine;

namespace DialogueSystem.Runtime.ScriptableObjects
{
    public class DialogueSystemDialogue : ScriptableObject
    {
        [field: SerializeField]
        [field: ReadOnly]
        public bool IsStartingDialogue { get; private set; }

        [field: SerializeField]
        [field: ReadOnly]
        public DialogueType Type { get; private set; }

        [field: SerializeField]
        [field: ReadOnly]
        public string Name { get; private set; }

        [field: SerializeField]
        [field: ReadOnly]
        [field: TextArea]
        public string Text { get; private set; }

        [field: SerializeField]
        [field: ReadOnly]
        public List<DialogueSystemDialogueChoiceData> Choices { get; private set; }

        public void Initialize(bool isStartingDialogue, DialogueType dialogueType, string dialogueName, string text, List<DialogueSystemDialogueChoiceData> choices)
        {
            IsStartingDialogue = isStartingDialogue;
            Type = dialogueType;
            Name = dialogueName;
            Text = text;
            Choices = choices;
        }
    }
}