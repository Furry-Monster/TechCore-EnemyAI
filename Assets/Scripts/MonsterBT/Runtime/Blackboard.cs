using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT.Runtime
{
    [Serializable]
    public class BlackboardKey
    {
        public string name;
        public Type valueType;

        public BlackboardKey(string name, Type valueType)
        {
            this.name = name;
            this.valueType = valueType;
        }
    }

    [CreateAssetMenu(fileName = "Blackboard", menuName = "MonsterBT/Blackboard")]
    public class Blackboard : ScriptableObject
    {
        [SerializeField] private Dictionary<string, object> data = new Dictionary<string, object>();

        public T GetValue<T>(string key)
        {
            if (data.TryGetValue(key, out object value))
            {
                if (value is T output)
                    return output;
            }
            return default(T);
        }

        public void SetValue<T>(string key, T value)
        {
            data[key] = value;
        }

        public bool HasKey(string key)
        {
            return data.ContainsKey(key);
        }

        public void RemoveKey(string key)
        {
            data.Remove(key);
        }

        public void Clear()
        {
            data.Clear();
        }

        // GameObject specific helpers
        public GameObject GetGameObject(string key) => GetValue<GameObject>(key);
        public void SetGameObject(string key, GameObject value) => SetValue(key, value);

        public Transform GetTransform(string key) => GetValue<Transform>(key);
        public void SetTransform(string key, Transform value) => SetValue(key, value);

        public Vector3 GetVector3(string key) => GetValue<Vector3>(key);
        public void SetVector3(string key, Vector3 value) => SetValue(key, value);

        public float GetFloat(string key) => GetValue<float>(key);
        public void SetFloat(string key, float value) => SetValue(key, value);

        public bool GetBool(string key) => GetValue<bool>(key);
        public void SetBool(string key, bool value) => SetValue(key, value);

        public string GetString(string key) => GetValue<string>(key);
        public void SetString(string key, string value) => SetValue(key, value);
    }
}