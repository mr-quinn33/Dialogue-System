using System;
using System.Collections.Generic;
using DialogueSystem.Runtime.Enumerations;
using UnityEngine;

namespace DialogueSystem.Editor.Data.Save
{
    [Serializable]
    public class DialogueSystemNodeSaveData
    {
        [field: SerializeField] public string ID { get; set; }

        [field: SerializeField] public string Name { get; set; }

        [field: SerializeField] public string Text { get; set; }

        [field: SerializeField] public List<DialogueSystemChoiceSaveData> Choices { get; set; }

        [field: SerializeField] public string GroupID { get; set; }

        [field: SerializeField] public DialogueType Type { get; set; }

        [field: SerializeField] public Vector2 Position { get; set; }
    }
}