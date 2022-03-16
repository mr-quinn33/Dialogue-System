using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DialogueSystem.Runtime.ScriptableObjects
{
    public class DialogueSystemDialogueContainer : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public string FileName { get; private set; }

        [field: SerializeField, ReadOnly]
        public SerializableDictionary<DialogueSystemDialogueGroup, List<DialogueSystemDialogue>> Groups
        {
            get;
            private set;
        }

        [field: SerializeField, ReadOnly] public List<DialogueSystemDialogue> UngroupedDialogues { get; private set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Groups = new SerializableDictionary<DialogueSystemDialogueGroup, List<DialogueSystemDialogue>>();
            UngroupedDialogues = new List<DialogueSystemDialogue>();
        }

        public List<string> GetDialogueGroupNames()
        {
            return Groups.Keys.Select(dialogueGroup => dialogueGroup.GroupName).ToList();
        }

        public List<string> GetGroupedDialogueNames(DialogueSystemDialogueGroup dialogueGroup,
            bool startingDialoguesOnly)
        {
            var groupedDialogues = Groups[dialogueGroup];
            return (from groupedDialogue in groupedDialogues
                where !startingDialoguesOnly || groupedDialogue.IsStartingDialogue
                select groupedDialogue.Name).ToList();
        }

        public List<string> GetUngroupedDialogueNames(bool startingDialoguesOnly)
        {
            return (from ungroupedDialogue in UngroupedDialogues
                where !startingDialoguesOnly || ungroupedDialogue.IsStartingDialogue
                select ungroupedDialogue.Name).ToList();
        }
    }
}