using System.IO;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Windows
{
    public class DialogueSystemEditorWindow : EditorWindow
    {
        private const string DefaultFileName = "DialogueFileName";
        private static TextField fileNameTextField;
        private DialogueSystemGraphView graphView;
        private Button saveButton;

        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();
            AddStyles();
        }

        [MenuItem("Window/Dialogue System/Dialogue Graph")]
        public static void Open()
        {
            _ = GetWindow<DialogueSystemEditorWindow>("Dialogue Graph");
        }

        private void AddGraphView()
        {
            graphView = new DialogueSystemGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        private void AddToolbar()
        {
            var toolbar = new Toolbar();
            fileNameTextField = DialogueSystemElementUtility.CreateTextField(DefaultFileName, "File Name:", callback =>
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters());
            saveButton = DialogueSystemElementUtility.CreateButton("Save", Save);
            var loadButton = DialogueSystemElementUtility.CreateButton("Load", Load);
            var clearButton = DialogueSystemElementUtility.CreateButton("Clear", Clear);
            var resetButton = DialogueSystemElementUtility.CreateButton("Reset", ResetGraph);
            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar = toolbar.AddStyleSheets("DialogueSystem/DialogueSystemToolbarStyles.uss") as Toolbar;
            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            _ = rootVisualElement.AddStyleSheets("DialogueSystem/DialogueSystemVariables.uss");
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                _ = EditorUtility.DisplayDialog("Invalid file name.", "Please ensure the file name you've typed in is valid.", "OK");
                return;
            }

            DialogueSystemIOUtility.Initialize(graphView, fileNameTextField.value);
            DialogueSystemIOUtility.Save();
        }

        private void Load()
        {
            var filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/DialogueSystem/Editor/Graphs", "asset");
            if (string.IsNullOrEmpty(filePath)) return;
            Clear();
            DialogueSystemIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
            DialogueSystemIOUtility.Load();
        }

        private void Clear()
        {
            graphView.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();
            UpdateFileName(DefaultFileName);
        }

        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
        }

        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }

        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
    }
}