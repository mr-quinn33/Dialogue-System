using System;
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
        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialoguesOnly;
        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;
        
        public string Text => dialogue ? dialogue.Text : null;
        
        public int ChoiceCount
        {
            get
            {
                if (!dialogue)
                {
                    return 0;
                }

                var dialogueType = dialogue.Type;
                return dialogueType switch
                {
                    DialogueType.SingleChoice => 1,
                    DialogueType.MultipleChoice => dialogue.Choices == null || dialogue.Choices.Count == 0
                        ? 0
                        : dialogue.Choices.Count,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
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
            if (!dialogue)
            {
                return null;
            }
            
            var choices = dialogue.Choices;
            if (choices == null)
            {
                return null;
            }
            
            var count = choices.Count;
            if (count == 0)
            {
                return null;
            }

            switch (dialogue.Type)
            {
                case DialogueType.SingleChoice:
                    return choices[0];
                case DialogueType.MultipleChoice:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            index = index < 0 ? 0 : index;
            index = index > count - 1 ? count - 1 : index;
            return choices[index];
        }
    }
}