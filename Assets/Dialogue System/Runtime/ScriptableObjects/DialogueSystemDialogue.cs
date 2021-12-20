using DialogueSystem.Runtime.Data;
using DialogueSystem.Runtime.Enumerations;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Runtime.ScriptableObjects
{
    public class DialogueSystemDialogue : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public bool IsStartingDialogue { get; private set; }

        [field: SerializeField, ReadOnly] public DialogueType Type { get; private set; }
        
        [field: SerializeField, ReadOnly] public string Name { get; private set; }

        [field: SerializeField, ReadOnly] [field: TextArea] public string Text { get; private set; }

        [field: SerializeField, ReadOnly] public List<DialogueSystemDialogueChoiceData> Choices { get; private set; }
        
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