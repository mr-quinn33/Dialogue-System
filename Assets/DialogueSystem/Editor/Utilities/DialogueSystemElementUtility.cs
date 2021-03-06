using System;
using DialogueSystem.Editor.Elements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Utilities
{
    public static class DialogueSystemElementUtility
    {
        public static Button CreateButton(string text, Action onClick = null)
        {
            var button = new Button(onClick)
            {
                text = text
            };
            return button;
        }

        public static Foldout CreateFoldout(string title, bool collapsed = false)
        {
            var foldout = new Foldout
            {
                text = title,
                value = !collapsed
            };
            return foldout;
        }

        public static Port CreatePort(this DialogueSystemNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            var port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
            port.portName = portName;
            return port;
        }

        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            var textField = new TextField
            {
                value = value,
                label = label
            };
            if (onValueChanged != null) _ = textField.RegisterValueChangedCallback(onValueChanged);
            return textField;
        }

        public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            var textArea = CreateTextField(value, label, onValueChanged);
            textArea.multiline = true;
            return textArea;
        }
    }
}