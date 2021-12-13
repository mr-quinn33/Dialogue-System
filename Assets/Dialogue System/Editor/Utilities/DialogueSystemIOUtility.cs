using DialogueSystem.Editor.Data.Save;
using DialogueSystem.Editor.Elements;
using DialogueSystem.Editor.Windows;
using DialogueSystem.Runtime.Data;
using DialogueSystem.Runtime.ScriptableObjects;
using DialogueSystem.Runtime.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace DialogueSystem.Editor.Utilities
{
    public static class DialogueSystemIOUtility
    {
        private static DialogueSystemGraphView graphView;
        private static string graphFileName;
        private static string containerFolderPath;
        private static List<DialogueSystemNode> nodes;
        private static List<DialogueSystemGroup> groups;
        private static Dictionary<string, DialogueSystemDialogueGroup> createdDialogueGroups;
        private static Dictionary<string, DialogueSystemDialogue> createdDialogues;
        private static Dictionary<string, DialogueSystemGroup> loadedGroups;
        private static Dictionary<string, DialogueSystemNode> loadedNodes;

        public static void Initialize(DialogueSystemGraphView dialogueSystemGraphView, string graphName)
        {
            graphView = dialogueSystemGraphView;
            graphFileName = graphName;
            containerFolderPath = $"Assets/Dialogue System/Dialogues/{graphName}";
            nodes = new List<DialogueSystemNode>();
            groups = new List<DialogueSystemGroup>();
            createdDialogueGroups = new Dictionary<string, DialogueSystemDialogueGroup>();
            createdDialogues = new Dictionary<string, DialogueSystemDialogue>();
            loadedGroups = new Dictionary<string, DialogueSystemGroup>();
            loadedNodes = new Dictionary<string, DialogueSystemNode>();
        }

        public static void Save()
        {
            CreateDefaultFolders();
            GetElementsFromGraphView();
            var graphData = CreateAsset<DialogueSystemGraphSaveData>("Assets/Dialogue System/Editor/Graphs", $"{graphFileName}");
            graphData.Initialize(graphFileName);
            var dialogueContainer = CreateAsset<DialogueSystemDialogueContainer>(containerFolderPath, graphFileName);
            dialogueContainer.Initialize(graphFileName);
            SaveGroups(graphData, dialogueContainer);
            SaveNodes(graphData, dialogueContainer);
            SaveAsset(graphData);
            SaveAsset(dialogueContainer);
        }

        private static void SaveGroups(DialogueSystemGraphSaveData graphData, DialogueSystemDialogueContainer dialogueContainer)
        {
            var groupNames = new List<string>();
            foreach (var group in groups)
            {
                SaveGroupToGraph(group, graphData);
                SaveGroupToScriptableObject(group, dialogueContainer);

                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }

        private static void SaveGroupToGraph(DialogueSystemGroup group, DialogueSystemGraphSaveData graphData)
        {
            var groupData = new DialogueSystemGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };
            graphData.Groups.Add(groupData);
        }

        private static void SaveGroupToScriptableObject(DialogueSystemGroup group, DialogueSystemDialogueContainer dialogueContainer)
        {
            var groupName = group.title;
            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");
            var dialogueGroup = CreateAsset<DialogueSystemDialogueGroup>($"{containerFolderPath}/Groups/{groupName}", groupName);
            dialogueGroup.Initialize(groupName);
            createdDialogueGroups.Add(group.ID, dialogueGroup);
            dialogueContainer.Groups.Add(dialogueGroup, new List<DialogueSystemDialogue>());
            SaveAsset(dialogueGroup);
        }

        private static void UpdateOldGroups(List<string> currentGroupNames, DialogueSystemGraphSaveData graphData)
        {
            if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
            {
                var groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();
                foreach (var groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }

        private static void SaveNodes(DialogueSystemGraphSaveData graphData, DialogueSystemDialogueContainer dialogueContainer)
        {
            var groupedNodeNames = new SerializableDictionary<string, List<string>>();
            var ungroupedNodeNames = new List<string>();
            foreach (var node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, dialogueContainer);
                if (node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.Name);

                    continue;
                }

                ungroupedNodeNames.Add(node.Name);
            }

            UpdateDialoguesChoicesConnections();
            UpdateOldGroupedNodes(groupedNodeNames, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        private static void SaveNodeToGraph(DialogueSystemNode node, DialogueSystemGraphSaveData graphData)
        {
            var choices = CloneNodeChoices(node.Choices);
            var nodeData = new DialogueSystemNodeSaveData()
            {
                ID = node.ID,
                Name = node.Name,
                Choices = choices,
                Text = node.Text,
                GroupID = node.Group?.ID,
                Type = node.Type,
                Position = node.GetPosition().position
            };
            graphData.Nodes.Add(nodeData);
        }

        private static void SaveNodeToScriptableObject(DialogueSystemNode node, DialogueSystemDialogueContainer dialogueContainer)
        {
            DialogueSystemDialogue dialogue;
            if (node.Group != null)
            {
                dialogue = CreateAsset<DialogueSystemDialogue>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.Name);
                dialogueContainer.Groups.AddItem(createdDialogueGroups[node.Group.ID], dialogue);
            }
            else
            {
                dialogue = CreateAsset<DialogueSystemDialogue>($"{containerFolderPath}/Global/Dialogues", node.Name);
                dialogueContainer.UngroupedDialogues.Add(dialogue);
            }

            dialogue.Initialize(node.Name, node.Text, ConvertNodeChoicesToDialogueChoices(node.Choices), node.Type, node.IsStartingNode());
            createdDialogues.Add(node.ID, dialogue);
            SaveAsset(dialogue);
        }

        private static List<DialogueSystemDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<DialogueSystemChoiceSaveData> nodeChoices)
        {
            var dialogueChoices = new List<DialogueSystemDialogueChoiceData>();
            foreach (var nodeChoice in nodeChoices)
            {
                var choiceData = new DialogueSystemDialogueChoiceData()
                {
                    Text = nodeChoice.Text
                };
                dialogueChoices.Add(choiceData);
            }

            return dialogueChoices;
        }

        private static void UpdateDialoguesChoicesConnections()
        {
            foreach (var node in nodes)
            {
                var dialogue = createdDialogues[node.ID];
                for (var choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                {
                    var nodeChoice = node.Choices[choiceIndex];
                    if (string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choiceIndex].NextDialogue = createdDialogues[nodeChoice.NodeID];
                    SaveAsset(dialogue);
                }
            }
        }

        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DialogueSystemGraphSaveData graphData)
        {
            if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
            {
                foreach (var oldGroupedNode in graphData.OldGroupedNodeNames)
                {
                    var nodesToRemove = new List<string>();
                    if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                    }

                    foreach (var nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                    }
                }
            }

            graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }

        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DialogueSystemGraphSaveData graphData)
        {
            if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
            {
                var nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();
                foreach (var nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
                }
            }

            graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
        }

        public static void Load()
        {
            var graphData = LoadAsset<DialogueSystemGraphSaveData>("Assets/Dialogue System/Editor/Graphs", graphFileName);
            if (graphData == null)
            {
                _ = EditorUtility.DisplayDialog(
                    "Could not find the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"\"Assets/Dialogue System/Editor/Graphs/{graphFileName}\".\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path mentioned above.",
                    "OK"
                );
                return;
            }

            DialogueSystemEditorWindow.UpdateFileName(graphData.FileName);
            LoadGroups(graphData.Groups);
            LoadNodes(graphData.Nodes);
            LoadNodesConnections();
        }

        private static void LoadGroups(List<DialogueSystemGroupSaveData> groupSaveDataList)
        {
            foreach (var groupData in groupSaveDataList)
            {
                var group = graphView.CreateGroup(groupData.Name, groupData.Position);
                group.ID = groupData.ID;
                loadedGroups.Add(group.ID, group);
            }
        }

        private static void LoadNodes(List<DialogueSystemNodeSaveData> nodeSaveDataList)
        {
            foreach (var nodeData in nodeSaveDataList)
            {
                var choices = CloneNodeChoices(nodeData.Choices);
                var node = graphView.CreateNode(nodeData.Name, nodeData.Type, nodeData.Position, false);
                node.ID = nodeData.ID;
                node.Choices = choices;
                node.Text = nodeData.Text;
                node.Draw();
                graphView.AddElement(node);
                loadedNodes.Add(node.ID, node);
                if (string.IsNullOrEmpty(nodeData.GroupID))
                {
                    continue;
                }

                var group = loadedGroups[nodeData.GroupID];
                node.Group = group;
                group.AddElement(node);
            }
        }

        private static void LoadNodesConnections()
        {
            foreach (var loadedNode in loadedNodes)
            {
                foreach (var visualElement in loadedNode.Value.outputContainer.Children())
                {
                    var choicePort = (Port) visualElement;
                    var choiceData = (DialogueSystemChoiceSaveData)choicePort.userData;
                    if (string.IsNullOrEmpty(choiceData.NodeID))
                    {
                        continue;
                    }

                    var nextNode = loadedNodes[choiceData.NodeID];
                    var nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();
                    var edge = choicePort.ConnectTo(nextNodeInputPort);
                    graphView.AddElement(edge);
                    _ = loadedNode.Value.RefreshPorts();
                }
            }
        }

        private static void CreateDefaultFolders()
        {
            CreateFolder("Assets", "Dialogue System");
            CreateFolder("Assets/Dialogue System", "Editor");
            CreateFolder("Assets/Dialogue System/Editor", "Graphs");
            CreateFolder("Assets/Dialogue System", "Dialogues");
            CreateFolder("Assets/Dialogue System/Dialogues", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Dialogues");
        }

        private static void GetElementsFromGraphView()
        {
            var groupType = typeof(DialogueSystemGroup);
            graphView.graphElements.ForEach(graphElement =>
            {
                if (graphElement is DialogueSystemNode node)
                {
                    nodes.Add(node);
                    return;
                }

                if (graphElement.GetType() == groupType)
                {
                    var group = (DialogueSystemGroup)graphElement;
                    groups.Add(group);
                }
            });
        }

        private static void CreateFolder(string parentFolderPath, string newFolderName)
        {
            if (AssetDatabase.IsValidFolder($"{parentFolderPath}/{newFolderName}"))
            {
                return;
            }

            _ = AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
        }

        private static void RemoveFolder(string path)
        {
            _ = FileUtil.DeleteFileOrDirectory($"{path}.meta");
            _ = FileUtil.DeleteFileOrDirectory($"{path}/");
        }

        private static T CreateAsset<T>(string path, string assetName) where T : UnityEngine.ScriptableObject
        {
            var fullPath = $"{path}/{assetName}.asset";
            var asset = LoadAsset<T>(path, assetName);
            if (asset == null)
            {
                asset = UnityEngine.ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }

        public static T LoadAsset<T>(string path, string assetName) where T : UnityEngine.ScriptableObject
        {
            var fullPath = $"{path}/{assetName}.asset";
            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        private static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void RemoveAsset(string path, string assetName)
        {
            _ = AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        private static List<DialogueSystemChoiceSaveData> CloneNodeChoices(List<DialogueSystemChoiceSaveData> nodeChoices)
        {
            return nodeChoices.Select(choice => new DialogueSystemChoiceSaveData {Text = choice.Text, NodeID = choice.NodeID}).ToList();
        }
    }
}