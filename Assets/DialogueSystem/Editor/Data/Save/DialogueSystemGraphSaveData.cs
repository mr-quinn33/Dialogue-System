using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Editor.Data.Save
{
    public class DialogueSystemGraphSaveData : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public string FileName { get; private set; }

        [field: SerializeField, ReadOnly] public List<DialogueSystemGroupSaveData> Groups { get; private set; }

        [field: SerializeField, ReadOnly] public List<DialogueSystemNodeSaveData> Nodes { get; private set; }

        [field: SerializeField, ReadOnly] public List<string> OldGroupNames { get; set; }

        [field: SerializeField, ReadOnly] public List<string> OldUngroupedNodeNames { get; set; }

        [field: SerializeField, ReadOnly] public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Groups = new List<DialogueSystemGroupSaveData>();
            Nodes = new List<DialogueSystemNodeSaveData>();
        }
    }
}