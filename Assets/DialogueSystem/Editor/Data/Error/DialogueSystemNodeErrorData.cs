using DialogueSystem.Editor.Elements;
using System.Collections.Generic;

namespace DialogueSystem.Editor.Data.Error
{
    public class DialogueSystemNodeErrorData
    {
        public DialogueSystemErrorData ErrorData { get; }

        public List<DialogueSystemNode> Nodes { get; }

        public DialogueSystemNodeErrorData()
        {
            ErrorData = new DialogueSystemErrorData();
            Nodes = new List<DialogueSystemNode>();
        }
    }
}