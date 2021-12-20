using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Runtime.ScriptableObjects
{
    public class DialogueSystemDialogueContainer : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public string FileName { get; private set; }

        [field: SerializeField, ReadOnly] public SerializableDictionary<DialogueSystemDialogueGroup, List<DialogueSystemDialogue>> Groups { get; private set; }

        [field: SerializeField, ReadOnly] public List<DialogueSystemDialogue> UngroupedDialogues { get; private set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Groups = new SerializableDictionary<DialogueSystemDialogueGroup, List<DialogueSystemDialogue>>();
            UngroupedDialogues = new List<DialogueSystemDialogue>();
        }

        public List<string> GetDialogueGroupNames()
        {
            var dialogueGroupNames = new List<string>();
            foreach (var dialogueGroup in Groups.Keys)
            {
                dialogueGroupNames.Add(dialogueGroup.GroupName);
            }

            return dialogueGroupNames;
        }

        public List<string> GetGroupedDialogueNames(DialogueSystemDialogueGroup dialogueGroup, bool startingDialoguesOnly)
        {
            var groupedDialogues = Groups[dialogueGroup];
            var groupedDialogueNames = new List<string>();
            foreach (var groupedDialogue in groupedDialogues)
            {
                if (startingDialoguesOnly && !groupedDialogue.IsStartingDialogue)
                {
                    continue;
                }

                groupedDialogueNames.Add(groupedDialogue.Name);
            }

            return groupedDialogueNames;
        }

        public List<string> GetUngroupedDialogueNames(bool startingDialoguesOnly)
        {
            var ungroupedDialogueNames = new List<string>();
            foreach (var ungroupedDialogue in UngroupedDialogues)
            {
                if (startingDialoguesOnly && !ungroupedDialogue.IsStartingDialogue)
                {
                    continue;
                }

                ungroupedDialogueNames.Add(ungroupedDialogue.Name);
            }

            return ungroupedDialogueNames;
        }
    }
}