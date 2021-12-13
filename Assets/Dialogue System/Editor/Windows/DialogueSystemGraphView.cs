using DialogueSystem.Editor.Data.Error;
using DialogueSystem.Editor.Data.Save;
using DialogueSystem.Editor.Elements;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Runtime.Enumerations;
using DialogueSystem.Runtime.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Windows
{
    public class DialogueSystemGraphView : GraphView
    {
        private int nameErrorsCount;
        private DialogueSystemSearchWindow searchWindow;
        private readonly DialogueSystemEditorWindow editorWindow;
        private readonly SerializableDictionary<string, DialogueSystemNodeErrorData> ungroupedNodes;
        private readonly SerializableDictionary<string, DialogueSystemGroupErrorData> groups;
        private readonly SerializableDictionary<Group, SerializableDictionary<string, DialogueSystemNodeErrorData>> groupedNodes;

        public int NameErrorsCount
        {
            get => nameErrorsCount;
            set
            {
                nameErrorsCount = value;
                if (nameErrorsCount == 0)
                {
                    editorWindow.EnableSaving();
                }

                if (nameErrorsCount == 1)
                {
                    editorWindow.DisableSaving();
                }
            }
        }

        public DialogueSystemGraphView(DialogueSystemEditorWindow dialogueSystemEditorWindow)
        {
            editorWindow = dialogueSystemEditorWindow;
            ungroupedNodes = new SerializableDictionary<string, DialogueSystemNodeErrorData>();
            groups = new SerializableDictionary<string, DialogueSystemGroupErrorData>();
            groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DialogueSystemNodeErrorData>>();
            AddManipulators();
            AddGridBackground();
            AddSearchWindow();
            AddStyles();
            RegisterOnElementsDeleted();
            RegisterOnGroupElementsAdded();
            RegisterOnGroupElementsRemoved();
            RegisterOnGroupRenamed();
            RegisterOnGraphViewChanged();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateNodeContextualMenu("Add Node/Single Choice", DialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node/Multiple Choice", DialogueType.MultipleChoice));
            this.AddManipulator(CreateGroupContextualMenu());
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueType dialogueType)
        {
            var contextualMenuManipulator = new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode("DialogueName", dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))));
            return contextualMenuManipulator;
        }

        private IManipulator CreateGroupContextualMenu()
        {
            var contextualMenuManipulator = new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))));
            return contextualMenuManipulator;
        }

        public DialogueSystemGroup CreateGroup(string title, Vector2 position)
        {
            var group = new DialogueSystemGroup(title, position);
            AddGroup(group);
            AddElement(group);
            foreach (GraphElement selectedElement in selection)
            {
                if (!(selectedElement is DialogueSystemNode))
                {
                    continue;
                }

                var node = (DialogueSystemNode)selectedElement;
                group.AddElement(node);
            }

            return group;
        }

        public DialogueSystemNode CreateNode(string nodeName, DialogueType dialogueType, Vector2 position, bool shouldDraw = true)
        {
            var nodeType = Type.GetType($"DialogueSystem.Editor.Elements.DialogueSystem{dialogueType}Node");
            var node = (DialogueSystemNode)Activator.CreateInstance(nodeType);
            node.Initialize(nodeName, this, position);
            if (shouldDraw)
            {
                node.Draw();
            }

            AddUngroupedNode(node);
            return node;
        }

        private void RegisterOnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                var groupType = typeof(DialogueSystemGroup);
                var edgeType = typeof(Edge);
                var groupsToDelete = new List<DialogueSystemGroup>();
                var nodesToDelete = new List<DialogueSystemNode>();
                var edgesToDelete = new List<Edge>();
                foreach (GraphElement selectedElement in selection)
                {
                    if (selectedElement is DialogueSystemNode node)
                    {
                        nodesToDelete.Add(node);
                        continue;
                    }

                    if (selectedElement.GetType() == edgeType)
                    {
                        var edge = (Edge)selectedElement;
                        edgesToDelete.Add(edge);
                        continue;
                    }

                    if (selectedElement.GetType() != groupType)
                    {
                        continue;
                    }

                    var group = (DialogueSystemGroup)selectedElement;
                    groupsToDelete.Add(group);
                }

                foreach (var groupToDelete in groupsToDelete)
                {
                    var groupNodes = new List<DialogueSystemNode>();
                    foreach (var groupElement in groupToDelete.containedElements)
                    {
                        if (!(groupElement is DialogueSystemNode))
                        {
                            continue;
                        }

                        var groupNode = (DialogueSystemNode)groupElement;
                        groupNodes.Add(groupNode);
                    }

                    groupToDelete.RemoveElements(groupNodes);
                    RemoveGroup(groupToDelete);
                    RemoveElement(groupToDelete);
                }

                DeleteElements(edgesToDelete);
                foreach (var nodeToDelete in nodesToDelete)
                {
                    if (nodeToDelete.Group != null)
                    {
                        nodeToDelete.Group.RemoveElement(nodeToDelete);
                    }

                    RemoveUngroupedNode(nodeToDelete);
                    nodeToDelete.DisconnectAllPorts();
                    RemoveElement(nodeToDelete);
                }
            };
        }

        private void RegisterOnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (var element in elements)
                {
                    if (!(element is DialogueSystemNode))
                    {
                        continue;
                    }

                    var dialogueSystemGroup = (DialogueSystemGroup)group;
                    var node = (DialogueSystemNode)element;
                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, dialogueSystemGroup);
                }
            };
        }

        private void RegisterOnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (var element in elements)
                {
                    if (!(element is DialogueSystemNode))
                    {
                        continue;
                    }

                    var dialogueSystemGroup = (DialogueSystemGroup)group;
                    var node = (DialogueSystemNode)element;
                    RemoveGroupedNode(node, dialogueSystemGroup);
                    AddUngroupedNode(node);
                }
            };
        }

        private void RegisterOnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                var dialogueSystemGroup = (DialogueSystemGroup)group;
                dialogueSystemGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();
                if (string.IsNullOrEmpty(dialogueSystemGroup.title))
                {
                    if (!string.IsNullOrEmpty(dialogueSystemGroup.OldTitle))
                    {
                        ++NameErrorsCount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dialogueSystemGroup.OldTitle))
                    {
                        --NameErrorsCount;
                    }
                }

                RemoveGroup(dialogueSystemGroup);
                dialogueSystemGroup.OldTitle = dialogueSystemGroup.title;
                AddGroup(dialogueSystemGroup);
            };
        }

        private void RegisterOnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (var edge in changes.edgesToCreate)
                    {
                        var nextNode = (DialogueSystemNode)edge.input.node;
                        var choiceData = (DialogueSystemChoiceSaveData)edge.output.userData;
                        choiceData.NodeID = nextNode.ID;
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    var edgeType = typeof(Edge);
                    foreach (var element in changes.elementsToRemove)
                    {
                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }

                        var edge = (Edge)element;
                        var choiceData = (DialogueSystemChoiceSaveData)edge.output.userData;
                        choiceData.NodeID = string.Empty;
                    }
                }

                return changes;
            };
        }

        public void AddUngroupedNode(DialogueSystemNode node)
        {
            var nodeName = node.DialogueName.ToLower();
            if (!ungroupedNodes.ContainsKey(nodeName))
            {
                var nodeErrorData = new DialogueSystemNodeErrorData();
                nodeErrorData.Nodes.Add(node);
                ungroupedNodes.Add(nodeName, nodeErrorData);
                return;
            }

            var ungroupedNodesList = ungroupedNodes[nodeName].Nodes;
            ungroupedNodesList.Add(node);
            var errorColor = ungroupedNodes[nodeName].ErrorData.Color;
            node.SetErrorStyle(errorColor);
            if (ungroupedNodesList.Count == 2)
            {
                ++NameErrorsCount;
                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveUngroupedNode(DialogueSystemNode node)
        {
            var nodeName = node.DialogueName.ToLower();
            var ungroupedNodesList = ungroupedNodes[nodeName].Nodes;
            _ = ungroupedNodesList.Remove(node);
            node.ResetStyle();
            if (ungroupedNodesList.Count == 1)
            {
                --NameErrorsCount;
                ungroupedNodesList[0].ResetStyle();
                return;
            }

            if (ungroupedNodesList.Count == 0)
            {
                _ = ungroupedNodes.Remove(nodeName);
            }
        }

        private void AddGroup(DialogueSystemGroup group)
        {
            var groupName = group.title.ToLower();
            if (!groups.ContainsKey(groupName))
            {
                var groupErrorData = new DialogueSystemGroupErrorData();
                groupErrorData.Groups.Add(group);
                groups.Add(groupName, groupErrorData);
                return;
            }

            var groupsList = groups[groupName].Groups;
            groupsList.Add(group);
            var errorColor = groups[groupName].ErrorData.Color;
            group.SetErrorStyle(errorColor);
            if (groupsList.Count == 2)
            {
                ++NameErrorsCount;
                groupsList[0].SetErrorStyle(errorColor);
            }
        }

        private void RemoveGroup(DialogueSystemGroup group)
        {
            var oldGroupName = group.OldTitle.ToLower();
            var groupsList = groups[oldGroupName].Groups;
            _ = groupsList.Remove(group);
            group.ResetStyle();
            if (groupsList.Count == 1)
            {
                --NameErrorsCount;
                groupsList[0].ResetStyle();
                return;
            }

            if (groupsList.Count == 0)
            {
                _ = groups.Remove(oldGroupName);
            }
        }

        public void AddGroupedNode(DialogueSystemNode node, DialogueSystemGroup group)
        {
            var nodeName = node.DialogueName.ToLower();
            node.Group = group;
            if (!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new SerializableDictionary<string, DialogueSystemNodeErrorData>());
            }

            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                var nodeErrorData = new DialogueSystemNodeErrorData();
                nodeErrorData.Nodes.Add(node);
                groupedNodes[group].Add(nodeName, nodeErrorData);
                return;
            }

            var groupedNodesList = groupedNodes[group][nodeName].Nodes;
            groupedNodesList.Add(node);
            var errorColor = groupedNodes[group][nodeName].ErrorData.Color;
            node.SetErrorStyle(errorColor);
            if (groupedNodesList.Count == 2)
            {
                ++NameErrorsCount;
                groupedNodesList[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveGroupedNode(DialogueSystemNode node, DialogueSystemGroup group)
        {
            var nodeName = node.DialogueName.ToLower();
            node.Group = null;
            var groupedNodesList = groupedNodes[group][nodeName].Nodes;
            _ = groupedNodesList.Remove(node);
            node.ResetStyle();
            if (groupedNodesList.Count == 1)
            {
                --NameErrorsCount;
                groupedNodesList[0].ResetStyle();
                return;
            }

            if (groupedNodesList.Count == 0)
            {
                _ = groupedNodes[group].Remove(nodeName);
                if (groupedNodes[group].Count == 0)
                {
                    _ = groupedNodes.Remove(group);
                }
            }
        }

        private void AddGridBackground()
        {
            var gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }

        private void AddSearchWindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<DialogueSystemSearchWindow>();
            }

            searchWindow.Initialize(this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        private void AddStyles()
        {
            var classNames = new string[]
            {
                "Dialogue System/DialogueSystemGraphViewStyles.uss",
                "Dialogue System/DialogueSystemNodeStyles.uss"
            };
            _ = this.AddStyleSheets(classNames);
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            var worldMousePosition = mousePosition;
            if (isSearchWindow)
            {
                worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, mousePosition - editorWindow.position.position);
            }

            var localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
        }

        public void ClearGraph()
        {
            graphElements.ForEach(graphElement => RemoveElement(graphElement));
            groups.Clear();
            groupedNodes.Clear();
            ungroupedNodes.Clear();
            NameErrorsCount = 0;
        }
    }
}