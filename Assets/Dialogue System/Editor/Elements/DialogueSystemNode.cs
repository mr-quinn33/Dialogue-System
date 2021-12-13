using DialogueSystem.Editor.Data.Save;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Windows;
using DialogueSystem.Runtime.Enumerations;
using DialogueSystem.Runtime.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
    public abstract class DialogueSystemNode : Node
    {
        private protected DialogueSystemGraphView graphView;
        private Color defaultBackgroundColor;

        public string ID { get; set; }

        public string Name { get; private set; }

        public List<DialogueSystemChoiceSaveData> Choices { get; set; }

        public string Text { get; set; }

        public DialogueType Type { get; protected set; }

        public DialogueSystemGroup Group { get; set; }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectPorts(inputContainer));
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectPorts(outputContainer));
            base.BuildContextualMenu(evt);
        }

        public virtual void Initialize(string nodeName, DialogueSystemGraphView dialogueSystemGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            Name = nodeName;
            Choices = new List<DialogueSystemChoiceSaveData>();
            Text = "Dialogue text.";
            SetPosition(new Rect(position, Vector2.zero));
            graphView = dialogueSystemGraphView;
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        public virtual void Draw()
        {
            var dialogueNameTextField = DialogueSystemElementUtility.CreateTextField(Name, null, callback =>
            {
                var target = (TextField)callback.target;
                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(Name))
                    {
                        ++graphView.NameErrorsCount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Name))
                    {
                        --graphView.NameErrorsCount;
                    }
                }

                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);
                    Name = target.value;
                    graphView.AddUngroupedNode(this);
                    return;
                }

                var currentGroup = Group;
                graphView.RemoveGroupedNode(this, Group);
                Name = target.value;
                graphView.AddGroupedNode(this, currentGroup);
            });
            var classNames = new[]
            {
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field"
            };
            _ = dialogueNameTextField.AddClasses(classNames);
            titleContainer.Insert(0, dialogueNameTextField);
            var inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(inputPort);
            var customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");
            var textFoldout = DialogueSystemElementUtility.CreateFoldout("Dialogue Text");
            var textTextField = DialogueSystemElementUtility.CreateTextArea(Text, null, callback => Text = callback.newValue);
            classNames = new[]
            {
                "ds-node__text-field",
                "ds-node__quote-text-field"
            };
            _ = textTextField.AddClasses(classNames);
            textFoldout.Add(textTextField);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
        }

        public void DisconnectAllPorts()
        {
            DisconnectPorts(inputContainer);
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement container)
        {
            foreach (var visualElement in container.Children())
            {
                var port = (Port) visualElement;
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public bool IsStartingNode()
        {
            var inputPort = (Port)inputContainer.Children().First();
            return !inputPort.connected;
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
    }
}