using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{
    /// <summary>
    /// 行为树黑板
    /// </summary>
    public class Blackboard : IDisposable
    {
        private Dictionary<string, BTVariable> SharedVariables = new();
        private bool notifyChanges = true;

        public void Add(BTVariable variable)
        {
            if (variable == null)
            {
                Debug.LogError("[MonsterBT] Cannot add null variable to blackboard");
                return;
            }

            if (string.IsNullOrEmpty(variable.Name))
            {
                Debug.LogError("[MonsterBT] Cannot add unnamed variable to blackboard");
                return;
            }

            if (SharedVariables.ContainsKey(variable.Name))
            {
                Debug.LogWarning($"[MonsterBT] Variable with name '{variable.Name}' already exists in blackboard. Overwriting.");
                SharedVariables[variable.Name] = variable;
            }
            else
            {
                SharedVariables.Add(variable.Name, variable);
            }
        }

        public void Remove(string name)
        {
            if (SharedVariables.ContainsKey(name))
            {
                SharedVariables.Remove(name);
            }
        }

        public bool SetValue(string name, object value)
        {
            if (!SharedVariables.TryGetValue(name, out BTVariable variable))
            {
                Debug.LogWarning($"[MonsterBT] Variable '{name}' not found in blackboard");
                return false;
            }

            object oldValue = variable.GetValue();
            variable.SetValue(value);

            // 如果启用了通知并且值发生了变化，则发送通知
            if (notifyChanges && !Equals(oldValue, value))
            {
                BehaviorTreeBus.Instance.PublishBlackboardChanged(name, value);
            }

            return true;
        }

        public BTVariable GetByName(string name)
        {
            SharedVariables.TryGetValue(name, out BTVariable result);
            return result;
        }

        public T GetByName<T>(string name) where T : BTVariable
        {
            if (SharedVariables.TryGetValue(name, out BTVariable result) && result is T typedVariable)
            {
                return typedVariable;
            }
            return null;
        }

        public T GetValue<T>(string name, T defaultValue = default)
        {
            if (!SharedVariables.TryGetValue(name, out BTVariable variable))
            {
                return defaultValue;
            }

            try
            {
                object value = variable.GetValue();
                if (value is T typedValue)
                {
                    return typedValue;
                }
                return defaultValue;
            }
            catch
            {
                Debug.LogError($"[MonsterBT] Error getting value for variable '{name}'");
                return defaultValue;
            }
        }

        public void EnableNotifications(bool enable)
        {
            notifyChanges = enable;
        }

        public bool HasVariable(string name)
        {
            return SharedVariables.ContainsKey(name);
        }

        public string[] GetAllVariableNames()
        {
            string[] names = new string[SharedVariables.Count];
            SharedVariables.Keys.CopyTo(names, 0);
            return names;
        }

        public void Clear()
        {
            SharedVariables.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}