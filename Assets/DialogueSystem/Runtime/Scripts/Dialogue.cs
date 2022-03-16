using System;
using System.Linq;
using DialogueSystem.Runtime.Data;
using DialogueSystem.Runtime.Enumerations;
using DialogueSystem.Runtime.ScriptableObjects;
using UnityEngine;

namespace DialogueSystem.Runtime.Scripts
{
    public class Dialogue : MonoBehaviour
    {
        [SerializeField] private DialogueSystemDialogueContainer dialogueContainer;
        [SerializeField] private DialogueSystemDialogueGroup dialogueGroup;
        [SerializeField] private DialogueSystemDialogue dialogue;
        [SerializeField] private bool isGroupedDialogues;
        [SerializeField] private bool isStartingDialogues;
        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;

        private DialogueSystemDialogue StartingDialogue
        {
            get
            {
                if (!dialogueContainer) return null;
                var ungroupedDialogues = dialogueContainer.UngroupedDialogues;
                foreach (var ungroupedDialogue in ungroupedDialogues.Where(ungroupedDialogue => ungroupedDialogue.IsStartingDialogue)) return ungroupedDialogue;
                var groups = dialogueContainer.Groups;
                return groups.SelectMany(pair => pair.Value.Where(dial => dial.IsStartingDialogue)).FirstOrDefault();
            }
        }

        public string Text => dialogue ? dialogue.Text : null;

        public string GroupName
        {
            get
            {
                var group = Group(dialogue);
                return group ? group.GroupName : null;
            }
        }

        public int ChoiceCount
        {
            get
            {
                if (!dialogue) return 0;
                var dialogueType = dialogue.Type;
                return dialogueType switch
                {
                    DialogueType.SingleChoice => 1,
                    DialogueType.MultipleChoice => dialogue.Choices == null || dialogue.Choices.Count == 0 ? 0 : dialogue.Choices.Count,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public void Reset()
        {
            dialogue = StartingDialogue;
        }

        public string ChoiceText(int index = 0)
        {
            var choice = Choice(index);
            return choice?.Text;
        }

        public void Choose(int index = 0)
        {
            var nextDialogue = ChoiceDialogue(index);
            dialogue = nextDialogue;
        }

        private DialogueSystemDialogue ChoiceDialogue(int index)
        {
            var choice = Choice(index);
            return choice?.NextDialogue;
        }

        private DialogueSystemDialogueChoiceData Choice(int index)
        {
            if (!dialogue) return null;
            var choices = dialogue.Choices;
            if (choices == null) return null;
            var count = choices.Count;
            if (count == 0) return null;
            switch (dialogue.Type)
            {
                case DialogueType.SingleChoice:
                {
                    return choices[0];
                }
                case DialogueType.MultipleChoice:
                {
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            index = index < 0 ? 0 : index;
            index = index > count - 1 ? count - 1 : index;
            return choices[index];
        }

        private DialogueSystemDialogueGroup Group(DialogueSystemDialogue dial)
        {
            if (!dial) return null;
            if (!dialogueContainer) return null;
            var groups = dialogueContainer.Groups;
            if (groups == null || groups.Count == 0) return null;
            return (from pair in groups where pair.Value.Contains(dial) select pair.Key).FirstOrDefault();
        }
    }
}