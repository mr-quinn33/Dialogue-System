using System.Collections.Generic;

namespace DialogueSystem.Runtime.Utilities
{
    public static class DialogueSystemCollectionUtility
    {
        public static void AddItem<T, K>(this SerializableDictionary<T, List<K>> serializableDictionary, T key, K value)
        {
            if (serializableDictionary.ContainsKey(key))
            {
                serializableDictionary[key].Add(value);
                return;
            }

            serializableDictionary.Add(key, new List<K>() { value });
        }
    }
}