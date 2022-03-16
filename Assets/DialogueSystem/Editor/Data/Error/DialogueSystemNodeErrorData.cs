using System.Collections.Generic;
using DialogueSystem.Editor.Elements;

namespace DialogueSystem.Editor.Data.Error
{
    public class DialogueSystemNodeErrorData
    {
        public DialogueSystemNodeErrorData()
        {
            ErrorData = new DialogueSystemErrorData();
            Nodes = new List<DialogueSystemNode>();
        }

        public DialogueSystemErrorData ErrorData { get; }

        public List<DialogueSystemNode> Nodes { get; }
    }
}