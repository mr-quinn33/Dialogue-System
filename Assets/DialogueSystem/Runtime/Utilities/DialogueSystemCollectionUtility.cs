using System.Collections.Generic;

namespace DialogueSystem.Runtime.Utilities
{
    public static class DialogueSystemCollectionUtility
    {
        public static void AddItem<K, V>(this SerializableDictionary<K, List<V>> serializableDictionary, K key, V value)
        {
            if (serializableDictionary.ContainsKey(key))
            {
                serializableDictionary[key].Add(value);
                return;
            }

            serializableDictionary.Add(key, new List<V> { value });
        }
    }
}