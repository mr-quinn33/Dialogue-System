using UnityEngine;

namespace DialogueSystem.Editor.Data.Error
{
    public class DialogueSystemErrorData
    {
        public DialogueSystemErrorData()
        {
            GenerateRandomColor();
        }

        public Color Color { get; private set; }

        private void GenerateRandomColor()
        {
            Color = new Color32((byte) Random.Range(65, 256), (byte) Random.Range(50, 176), (byte) Random.Range(50, 176), 255);
        }
    }
}