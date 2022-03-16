using System.Collections.Generic;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Runtime.ScriptableObjects;
using DialogueSystem.Runtime.Scripts;
using UnityEditor;

namespace DialogueSystem.Editor.Inspectors
{
    [CustomEditor(typeof(Dialogue))]
    public class DialogueInspector : UnityEditor.Editor
    {
        private SerializedProperty dialogueContainerProperty;
        private SerializedProperty dialogueGroupProperty;
        private SerializedProperty dialogueProperty;
        private SerializedProperty isGroupedDialoguesProperty;
        private SerializedProperty isStartingDialoguesProperty;
        private SerializedProperty selectedDialogueGroupIndexProperty;
        private SerializedProperty selectedDialogueIndexProperty;

        private void OnEnable()
        {
            dialogueContainerProperty = serializedObject.FindProperty("dialogueContainer");
            dialogueGroupProperty = serializedObject.FindProperty("dialogueGroup");
            dialogueProperty = serializedObject.FindProperty("dialogue");
            isGroupedDialoguesProperty = serializedObject.FindProperty("isGroupedDialogues");
            isStartingDialoguesProperty = serializedObject.FindProperty("isStartingDialogues");
            selectedDialogueGroupIndexProperty = serializedObject.FindProperty("selectedDialogueGroupIndex");
            selectedDialogueIndexProperty = serializedObject.FindProperty("selectedDialogueIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDialogueContainerArea();
            var currentDialogueContainer =
                (DialogueSystemDialogueContainer) dialogueContainerProperty.objectReferenceValue;
            if (currentDialogueContainer == null)
            {
                StopDrawing("Select a Dialogue Container to see the rest of the Inspector.");
                return;
            }

            DrawFiltersArea();
            var currentGroupedDialoguesOnlyFilter = isGroupedDialoguesProperty.boolValue;
            var currentStartingDialoguesOnlyFilter = isStartingDialoguesProperty.boolValue;
            List<string> dialogueNames;
            var dialogueFolderPath = $"Assets/DialogueSystem/Dialogues/{currentDialogueContainer.FileName}";
            string dialogueInfoMessage;
            if (currentGroupedDialoguesOnlyFilter)
            {
                var dialogueGroupNames = currentDialogueContainer.GetDialogueGroupNames();
                if (dialogueGroupNames.Count == 0)
                {
                    StopDrawing("There are no Dialogue Groups in this Dialogue Container.");
                    return;
                }

                DrawDialogueGroupArea(currentDialogueContainer, dialogueGroupNames);
                var dialogueGroup = (DialogueSystemDialogueGroup) dialogueGroupProperty.objectReferenceValue;
                dialogueNames =
                    currentDialogueContainer.GetGroupedDialogueNames(dialogueGroup, currentStartingDialoguesOnlyFilter);
                dialogueFolderPath += $"/Groups/{dialogueGroup.GroupName}/Dialogues";
                dialogueInfoMessage = "There are no" +
                                      (currentStartingDialoguesOnlyFilter ? " Starting" : string.Empty) +
                                      " Dialogues in this Dialogue Group.";
            }
            else
            {
                dialogueNames = currentDialogueContainer.GetUngroupedDialogueNames(currentStartingDialoguesOnlyFilter);
                dialogueFolderPath += "/Global/Dialogues";
                dialogueInfoMessage = "There are no" +
                                      (currentStartingDialoguesOnlyFilter ? " Starting" : string.Empty) +
                                      " Ungrouped Dialogues in this Dialogue Container.";
            }

            if (dialogueNames.Count == 0)
            {
                StopDrawing(dialogueInfoMessage);
                return;
            }

            DrawDialogueArea(dialogueNames, dialogueFolderPath);
            _ = serializedObject.ApplyModifiedProperties();
        }

        private void DrawDialogueContainerArea()
        {
            DialogueSystemEditorUtility.DrawHeader("Dialogue Container");
            _ = dialogueContainerProperty.DrawPropertyField();
            DialogueSystemEditorUtility.DrawSpace();
        }

        private void DrawFiltersArea()
        {
            DialogueSystemEditorUtility.DrawHeader("Filters");
            _ = isGroupedDialoguesProperty.DrawPropertyField();
            _ = isStartingDialoguesProperty.DrawPropertyField();
            DialogueSystemEditorUtility.DrawSpace();
        }

        private void DrawDialogueGroupArea(DialogueSystemDialogueContainer dialogueContainer,
            List<string> dialogueGroupNames)
        {
            DialogueSystemEditorUtility.DrawHeader("Dialogue Group");
            var oldSelectedDialogueGroupIndex = selectedDialogueGroupIndexProperty.intValue;
            var oldDialogueGroup = (DialogueSystemDialogueGroup) dialogueGroupProperty.objectReferenceValue;
            var isOldDialogueGroupNull = oldDialogueGroup == null;
            var oldDialogueGroupName = isOldDialogueGroupNull ? string.Empty : oldDialogueGroup.GroupName;
            UpdateIndexOnNamesListUpdate(dialogueGroupNames, selectedDialogueGroupIndexProperty,
                oldSelectedDialogueGroupIndex, oldDialogueGroupName, isOldDialogueGroupNull);
            selectedDialogueGroupIndexProperty.intValue = DialogueSystemEditorUtility.DrawPopup("Dialogue Group",
                selectedDialogueGroupIndexProperty, dialogueGroupNames.ToArray());
            var selectedDialogueGroupName = dialogueGroupNames[selectedDialogueGroupIndexProperty.intValue];
            var selectedDialogueGroup = DialogueSystemIOUtility.LoadAsset<DialogueSystemDialogueGroup>(
                $"Assets/DialogueSystem/Dialogues/{dialogueContainer.FileName}/Groups/{selectedDialogueGroupName}",
                selectedDialogueGroupName);
            dialogueGroupProperty.objectReferenceValue = selectedDialogueGroup;
            DialogueSystemEditorUtility.DrawDisabledFields(() => dialogueGroupProperty.DrawPropertyField());
            DialogueSystemEditorUtility.DrawSpace();
        }

        private void DrawDialogueArea(List<string> dialogueNames, string dialogueFolderPath)
        {
            DialogueSystemEditorUtility.DrawHeader("Dialogue");
            var oldSelectedDialogueIndex = selectedDialogueIndexProperty.intValue;
            var oldDialogue = (DialogueSystemDialogue) dialogueProperty.objectReferenceValue;
            var isOldDialogueNull = oldDialogue == null;
            var oldDialogueName = isOldDialogueNull ? string.Empty : oldDialogue.Name;
            UpdateIndexOnNamesListUpdate(dialogueNames, selectedDialogueIndexProperty, oldSelectedDialogueIndex,
                oldDialogueName, isOldDialogueNull);
            selectedDialogueIndexProperty.intValue = DialogueSystemEditorUtility.DrawPopup("Dialogue",
                selectedDialogueIndexProperty, dialogueNames.ToArray());
            var selectedDialogueName = dialogueNames[selectedDialogueIndexProperty.intValue];
            var selectedDialogue =
                DialogueSystemIOUtility.LoadAsset<DialogueSystemDialogue>(dialogueFolderPath, selectedDialogueName);
            dialogueProperty.objectReferenceValue = selectedDialogue;
            DialogueSystemEditorUtility.DrawDisabledFields(() => dialogueProperty.DrawPropertyField());
        }

        private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
        {
            DialogueSystemEditorUtility.DrawHelpBox(reason, messageType);
            DialogueSystemEditorUtility.DrawSpace();
            DialogueSystemEditorUtility.DrawHelpBox(
                "You need to select a Dialogue for this component to work properly at Runtime!", MessageType.Warning);
            _ = serializedObject.ApplyModifiedProperties();
        }

        private static void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty,
            int oldSelectedPropertyIndex, string oldPropertyName, bool isOldPropertyNull)
        {
            if (isOldPropertyNull)
            {
                indexProperty.intValue = 0;
                return;
            }

            var oldIndexIsOutOfBoundsOfNamesListCount = oldSelectedPropertyIndex > optionNames.Count - 1;
            var oldNameIsDifferentThanSelectedName = oldIndexIsOutOfBoundsOfNamesListCount ||
                                                     oldPropertyName != optionNames[oldSelectedPropertyIndex];
            if (!oldNameIsDifferentThanSelectedName)
            {
                return;
            }

            if (optionNames.Contains(oldPropertyName))
            {
                indexProperty.intValue = optionNames.IndexOf(oldPropertyName);
                return;
            }

            indexProperty.intValue = 0;
        }
    }
}