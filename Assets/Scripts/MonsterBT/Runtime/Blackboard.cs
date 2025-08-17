using System;
using System.Collections.Generic;
using MonsterBT.Runtime.Utils;
using UnityEngine;

namespace MonsterBT.Runtime
{
    [Serializable]
    public struct BlackboardVariableInfo
    {
        public string name;
        public string typeName;
        public bool isExposed; // 是否在编辑器中暴露
    }

    [CreateAssetMenu(fileName = "Blackboard", menuName = "MonsterBT/Blackboard")]
    public class Blackboard : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<string, object> data =
            new SerializableDictionary<string, object>();
        
        [SerializeField] private List<BlackboardVariableInfo> variableInfos = 
            new List<BlackboardVariableInfo>();

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

#if UNITY_EDITOR
        public IReadOnlyList<BlackboardVariableInfo> GetVariableInfos() => variableInfos;
        
        public void AddVariable(string name, Type type, object defaultValue = null, bool isExposed = true)
        {
            if (HasKey(name)) return;
            
            SetValue(name, defaultValue);
            
            var info = new BlackboardVariableInfo
            {
                name = name,
                typeName = type.AssemblyQualifiedName,
                isExposed = isExposed
            };
            
            variableInfos.Add(info);
        }
        
        public void RemoveVariable(string name)
        {
            RemoveKey(name);
            variableInfos.RemoveAll(info => info.name == name);
        }
        
        public void RenameVariable(string oldName, string newName)
        {
            if (!HasKey(oldName) || HasKey(newName)) return;
            
            var value = data[oldName];
            RemoveKey(oldName);
            SetValue(newName, value);
            
            for (int i = 0; i < variableInfos.Count; i++)
            {
                if (variableInfos[i].name == oldName)
                {
                    var info = variableInfos[i];
                    info.name = newName;
                    variableInfos[i] = info;
                    break;
                }
            }
        }
        
        public Type GetVariableType(string name)
        {
            var info = variableInfos.Find(v => v.name == name);
            return string.IsNullOrEmpty(info.typeName) ? null : Type.GetType(info.typeName);
        }
        
        public IEnumerable<string> GetAllKeys()
        {
            return data.Keys;
        }
#endif
    }
}