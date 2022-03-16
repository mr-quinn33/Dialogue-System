using DialogueSystem.Editor.Data.Save;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Windows;
using DialogueSystem.Runtime.Enumerations;
using UnityEngine;

namespace DialogueSystem.Editor.Elements
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DialogueSystemSingleChoiceNode : DialogueSystemNode
    {
        public override void Initialize(string nodeName, DialogueSystemGraphView dialogueSystemGraphView,
            Vector2 position)
        {
            base.Initialize(nodeName, dialogueSystemGraphView, position);
            Type = DialogueType.SingleChoice;
            var choiceData = new DialogueSystemChoiceSaveData
            {
                Text = "Next Dialogue"
            };
            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();
            foreach (var choice in Choices)
            {
                var choicePort = this.CreatePort(choice.Text);
                choicePort.userData = choice;
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}