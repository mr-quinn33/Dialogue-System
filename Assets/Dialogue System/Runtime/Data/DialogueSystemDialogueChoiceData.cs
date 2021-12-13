using DialogueSystem.Runtime.ScriptableObjects;
using System;
using UnityEngine;

namespace DialogueSystem.Runtime.Data
{
    [Serializable]
    public class DialogueSystemDialogueChoiceData
    {
        [field: SerializeField] public string Text { get; set; }

        [field: SerializeField] public DialogueSystemDialogue NextDialogue { get; set; }
    }
}