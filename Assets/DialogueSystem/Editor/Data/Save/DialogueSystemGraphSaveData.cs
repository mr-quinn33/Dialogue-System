using System.Collections.Generic;
using DialogueSystem.Runtime;
using UnityEngine;

namespace DialogueSystem.Editor.Data.Save
{
    public class DialogueSystemGraphSaveData : ScriptableObject
    {
        [field: SerializeField]
        [field: ReadOnly]
        public string FileName { get; private set; }

        [field: SerializeField]
        [field: ReadOnly]
        public List<DialogueSystemGroupSaveData> Groups { get; private set; }

        [field: SerializeField]
        [field: ReadOnly]
        public List<DialogueSystemNodeSaveData> Nodes { get; private set; }

        [field: SerializeField]
        [field: ReadOnly]
        public List<string> OldGroupNames { get; set; }

        [field: SerializeField]
        [field: ReadOnly]
        public List<string> OldUngroupedNodeNames { get; set; }

        [field: SerializeField]
        [field: ReadOnly]
        public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Groups = new List<DialogueSystemGroupSaveData>();
            Nodes = new List<DialogueSystemNodeSaveData>();
        }
    }
}