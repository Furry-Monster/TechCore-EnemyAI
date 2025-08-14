using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace MonsterBT.Runtime.Utils
{
    [Serializable]
    public struct SerializablePair<TKey, TValue>
    {
        public TKey key;
        public TValue value;
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> :
        ICollection<SerializablePair<TKey, TValue>>
    {
        // 序列化字典，用于显示在Inspector中
        [SerializeField]
        private List<SerializablePair<TKey, TValue>> serializableDict = new List<SerializablePair<TKey, TValue>>();

        // 运行时字典，用于处理数据
        private Dictionary<TKey, TValue> rawDict;

        public int Count => rawDict.Count;
        public bool IsReadOnly { get; } = false;
        public ICollection<TKey> Keys => rawDict.Keys;
        public ICollection<TValue> Values => rawDict.Values;

        public TValue this[TKey key]
        {
            get => rawDict[key];
            set
            {
                rawDict[key] = value;
                SyncToSerializable();
            }
        }

        public SerializableDictionary()
        {
            rawDict = new Dictionary<TKey, TValue>();
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            rawDict = new Dictionary<TKey, TValue>(dictionary);
            SyncToSerializable();
        }

        public void Add(TKey key, TValue value)
        {
            rawDict.Add(key, value);
            SyncToSerializable();
        }

        public bool Remove(TKey key)
        {
            bool removed = rawDict.Remove(key);
            if (removed)
                SyncToSerializable();
            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return rawDict.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            return rawDict.ContainsKey(key);
        }

        public IEnumerator<SerializablePair<TKey, TValue>> GetEnumerator()
        {
            return rawDict
                .Select(kvp => new SerializablePair<TKey, TValue> { key = kvp.Key, value = kvp.Value })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(SerializablePair<TKey, TValue> item)
        {
            Add(item.key, item.value);
        }

        public void Clear()
        {
            rawDict.Clear();
            serializableDict.Clear();
        }

        public bool Contains(SerializablePair<TKey, TValue> item)
        {
            return rawDict.TryGetValue(item.key, out TValue value) &&
                   EqualityComparer<TValue>.Default.Equals(value, item.value);
        }

        public void CopyTo(SerializablePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Array is too small");

            int index = arrayIndex;
            foreach (var kvp in rawDict)
            {
                array[index++] = new SerializablePair<TKey, TValue> { key = kvp.Key, value = kvp.Value };
            }
        }

        public bool Remove(SerializablePair<TKey, TValue> item)
        {
            if (rawDict.TryGetValue(item.key, out TValue value) &&
                EqualityComparer<TValue>.Default.Equals(value, item.value))
            {
                return Remove(item.key);
            }
            return false;
        }

        /// <summary>
        /// 同步到序列化字典,耗时
        /// </summary>
        private void SyncToSerializable()
        {
            serializableDict.Clear();
            foreach (var kvp in rawDict)
            {
                serializableDict.Add(new SerializablePair<TKey, TValue> { key = kvp.Key, value = kvp.Value });
            }
        }

        /// <summary>
        /// 同步到运行时字典,耗时
        /// </summary>
        private void SyncFromSerializable()
        {
            rawDict.Clear();
            foreach (var pair in serializableDict)
            {
                if (!rawDict.ContainsKey(pair.key))
                {
                    rawDict[pair.key] = pair.value;
                }
            }
        }
    }
}