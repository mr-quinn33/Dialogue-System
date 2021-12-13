using UnityEngine;

namespace DialogueSystem.Editor.Data.Error
{
    public class DialogueSystemErrorData
    {
        public Color Color { get; set; }

        public DialogueSystemErrorData()
        {
            GenerateRandomColor();
        }

        private void GenerateRandomColor()
        {
            Color = new Color32((byte)Random.Range(65, 256), (byte)Random.Range(50, 176), (byte)Random.Range(50, 176), 255);
        }
    }
}