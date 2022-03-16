using System.Collections.Generic;
using DialogueSystem.Editor.Elements;

namespace DialogueSystem.Editor.Data.Error
{
    public class DialogueSystemGroupErrorData
    {
        public DialogueSystemGroupErrorData()
        {
            ErrorData = new DialogueSystemErrorData();
            Groups = new List<DialogueSystemGroup>();
        }

        public DialogueSystemErrorData ErrorData { get; }

        public List<DialogueSystemGroup> Groups { get; }
    }
}