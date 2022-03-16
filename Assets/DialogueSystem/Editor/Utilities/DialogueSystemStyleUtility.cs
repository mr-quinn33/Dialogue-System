using UnityEditor;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Utilities
{
    public static class DialogueSystemStyleUtility
    {
        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (var className in classNames) element.AddToClassList(className);
            return element;
        }

        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (var styleSheetName in styleSheetNames)
            {
                var styleSheet = (StyleSheet) EditorGUIUtility.Load(styleSheetName);
                element.styleSheets.Add(styleSheet);
            }

            return element;
        }
    }
}