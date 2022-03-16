using System.Collections.Generic;

namespace DialogueSystem.Runtime.Utilities
{
    public static class DialogueSystemCollectionUtility
    {
        public static void AddItem<TKey, TValue>(this SerializableDictionary<TKey, List<TValue>> serializableDictionary, TKey key, TValue value)
        {
            if (serializableDictionary.ContainsKey(key))
            {
                serializableDictionary[key].Add(value);
                return;
            }

            serializableDictionary.Add(key, new List<TValue> {value});
        }
    }
}