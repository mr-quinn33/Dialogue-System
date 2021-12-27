using DialogueSystem.Editor.Elements;
using System;
using System.Collections.Generic;
using DialogueSystem.Runtime.Enumerations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.Editor.Windows
{
    public class DialogueSystemSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueSystemGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(DialogueSystemGraphView dialogueSystemGraphView)
        {
            graphView = dialogueSystemGraphView;
            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var searchTreeEntries = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Single Choice", indentationIcon))
                {
                    userData = DialogueType.SingleChoice,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
                {
                    userData = DialogueType.MultipleChoice,
                    level = 2
                },
                new SearchTreeGroupEntry(new GUIContent("Dialogue Groups"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                {
                    userData = new Group(),
                    level = 2
                }
            };
            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);
            return (searchTreeEntry.userData) switch
            {
                DialogueType.SingleChoice => Invoke(() =>
                {
                    var singleChoiceNode = (DialogueSystemSingleChoiceNode)graphView.CreateNode("DialogueName", DialogueType.SingleChoice, localMousePosition);
                    graphView.AddElement(singleChoiceNode);
                    return true;
                }),
                DialogueType.MultipleChoice => Invoke(() =>
                {
                    var multipleChoiceNode = (DialogueSystemMultipleChoiceNode)graphView.CreateNode("DialogueName", DialogueType.MultipleChoice, localMousePosition);
                    graphView.AddElement(multipleChoiceNode);
                    return true;
                }),
                Group _ => Invoke(() =>
                {
                    _ = graphView.CreateGroup("DialogueGroup", localMousePosition);
                    return true;
                }),
                _ => false
            };
        }

        private static T Invoke<T>(Func<T> func)
        {
            return func.Invoke();
        }
    }
}