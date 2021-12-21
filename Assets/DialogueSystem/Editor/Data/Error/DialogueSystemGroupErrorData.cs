using DialogueSystem.Editor.Elements;
using System.Collections.Generic;

namespace DialogueSystem.Editor.Data.Error
{
    public class DialogueSystemGroupErrorData
    {
        public DialogueSystemErrorData ErrorData { get; }

        public List<DialogueSystemGroup> Groups { get; }

        public DialogueSystemGroupErrorData()
        {
            ErrorData = new DialogueSystemErrorData();
            Groups = new List<DialogueSystemGroup>();
        }
    }
}