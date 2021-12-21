using System.Linq;
using DialogueSystem.Editor.Data.Save;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Windows;
using DialogueSystem.Runtime.Enumerations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DialogueSystemMultipleChoiceNode : DialogueSystemNode
    {
        public override void Initialize(string nodeName, DialogueSystemGraphView dialogueSystemGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dialogueSystemGraphView, position);
            Type = DialogueType.MultipleChoice;
            var choiceData = new DialogueSystemChoiceSaveData
            {
                Text = "New Choice"
            };
            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();
            var addChoiceButton = DialogueSystemElementUtility.CreateButton("Add Choice", () =>
            {
                var choiceData = new DialogueSystemChoiceSaveData
                {
                    Text = "New Choice"
                };
                Choices.Add(choiceData);
                var choicePort = CreateChoicePort(choiceData);
                outputContainer.Add(choicePort);
            });
            addChoiceButton.AddToClassList("ds-node__button");
            mainContainer.Insert(1, addChoiceButton);
            foreach (var choicePort in Choices.Select(CreateChoicePort))
            {
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

        private Port CreateChoicePort(object data)
        {
            var choicePort = this.CreatePort();
            choicePort.userData = data;
            var choiceData = (DialogueSystemChoiceSaveData)data;
            var deleteChoiceButton = DialogueSystemElementUtility.CreateButton("Delete", () =>
            {
                if (Choices.Count == 1)
                {
                    return;
                }

                if (choicePort.connected)
                {
                    GraphView.DeleteElements(choicePort.connections);
                }

                _ = Choices.Remove(choiceData);
                GraphView.RemoveElement(choicePort);
            });
            deleteChoiceButton.AddToClassList("ds-node__button");
            var choiceTextField = DialogueSystemElementUtility.CreateTextField(choiceData.Text, null, callback => choiceData.Text = callback.newValue);
            var classNames = new[]
            {
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__choice-text-field"
            };
            choiceTextField = choiceTextField.AddClasses(classNames) as TextField;
            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            return choicePort;
        }
    }
}