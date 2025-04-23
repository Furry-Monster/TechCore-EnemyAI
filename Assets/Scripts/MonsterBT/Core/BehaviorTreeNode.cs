using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{

    public enum NodeState
    {
        Executing,      // 节点执行中
        Success,        // 执行成功
        Failure,        // 执行失败
        Error,          // 执行错误,需要Debug
    };

    public abstract class BehaviorTreeNode : IDisposable
    {
        protected BehaviorTree Tree { get; private set; }
        protected BehaviorTreeExec Exec { get; private set; }
        protected Action<NodeState> OnStateChanged;

        public void Initalize(BehaviorTree tree, BehaviorTreeExec exec)
        {
            Tree = tree;
            Exec = exec;

            OnInitialize();
        }

        protected abstract void OnInitialize();

        public NodeState Execute()
        {
            var state = DoExecute();

            Debug.Log($"Now exec node : {GetType()}");

            OnStateChanged?.Invoke(state);

            return state;
        }

        protected abstract NodeState DoExecute();

        public virtual void Dispose()
        {
            OnStateChanged = null;
        }
    }
}