using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{
    public enum LogLevel
    {
        Info,
        Warn,
        Error
    }

    public class Log : Action
    {
        public string Message;
        public LogLevel Level;

        protected override NodeState DoExecute()
        {
            switch (Level)
            {
                case LogLevel.Info:
                    Debug.Log($"[MonsterBT] {Message}");
                    break;
                case LogLevel.Warn:
                    Debug.LogWarning($"[MonsterBT] {Message}");
                    break;
                case LogLevel.Error:
                    Debug.LogError($"[MonsterBT] {Message}");
                    break;
                default:
                    Debug.LogWarning($"[MonsterBT] {Message} (Default LogLevel)");
                    break;
            }

            return NodeState.Success;
        }
    }
}