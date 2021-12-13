using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Runtime.ScriptableObjects
{
    public class DialogueSystemDialogueContainer : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }

        [field: SerializeField] public SerializableDictionary<DialogueSystemDialogueGroup, List<DialogueSystemDialogue>> DialogueGroups { get; set; }

        [field: SerializeField] public List<DialogueSystemDialogue> UngroupedDialogues { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            DialogueGroups = new SerializableDictionary<DialogueSystemDialogueGroup, List<DialogueSystemDialogue>>();
            UngroupedDialogues = new List<DialogueSystemDialogue>();
        }

        public List<string> GetDialogueGroupNames()
        {
            var dialogueGroupNames = new List<string>();
            foreach (var dialogueGroup in DialogueGroups.Keys)
            {
                dialogueGroupNames.Add(dialogueGroup.GroupName);
            }

            return dialogueGroupNames;
        }

        public List<string> GetGroupedDialogueNames(DialogueSystemDialogueGroup dialogueGroup, bool startingDialoguesOnly)
        {
            var groupedDialogues = DialogueGroups[dialogueGroup];
            var groupedDialogueNames = new List<string>();
            foreach (var groupedDialogue in groupedDialogues)
            {
                if (startingDialoguesOnly && !groupedDialogue.IsStartingDialogue)
                {
                    continue;
                }

                groupedDialogueNames.Add(groupedDialogue.DialogueName);
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

                ungroupedDialogueNames.Add(ungroupedDialogue.DialogueName);
            }

            return ungroupedDialogueNames;
        }
    }
}