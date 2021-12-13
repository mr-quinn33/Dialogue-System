using System;
using UnityEngine;

namespace DialogueSystem.Editor.Data.Save
{
    [Serializable]
    public class DialogueSystemChoiceSaveData
    {
        [field: SerializeField] public string Text { get; set; }

        [field: SerializeField] public string NodeID { get; set; }
    }
}