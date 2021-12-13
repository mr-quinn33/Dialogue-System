using DialogueSystem.Editor.Elements;
using System.Collections.Generic;

namespace DialogueSystem.Editor.Data.Error
{
    public class DialogueSystemNodeErrorData
    {
        public DialogueSystemErrorData ErrorData { get; set; }

        public List<DialogueSystemNode> Nodes { get; set; }

        public DialogueSystemNodeErrorData()
        {
            ErrorData = new DialogueSystemErrorData();
            Nodes = new List<DialogueSystemNode>();
        }
    }
}