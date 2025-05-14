using System;
using System.Collections.Generic;

namespace MonsterBT
{
    public class Blackboard : IDisposable
    {
        private Dictionary<string, BTVariable> SharedVariables = new();

        public void Add(BTVariable variable)
        {
            SharedVariables.Add(variable.Name, variable);
        }

        public void Remove(string name)
        {
            SharedVariables.Remove(name);
        }

        public BTVariable GetByName(string name)
        {
            return SharedVariables[name] ?? null;
        }

        public T GetByName<T>(string name) where T : BTVariable
        {
            BTVariable result = SharedVariables[name];
            if (result is T)
                return result as T;
            else
                return null;
        }

        public void Dispose()
        {
            SharedVariables.Clear();
        }
    }
}