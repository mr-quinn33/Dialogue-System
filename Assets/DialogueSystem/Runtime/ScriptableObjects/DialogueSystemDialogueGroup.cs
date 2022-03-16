using UnityEngine;

namespace DialogueSystem.Runtime.ScriptableObjects
{
    public class DialogueSystemDialogueGroup : ScriptableObject
    {
        [field: SerializeField]
        [field: ReadOnly]
        public string GroupName { get; private set; }

        public void Initialize(string groupName)
        {
            GroupName = groupName;
        }
    }
}