using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{

    public enum NodeState
    {
        Executing,      // �ڵ�ִ����
        Success,        // ִ�гɹ�
        Failure,        // ִ��ʧ��
        Error,          // ִ�д���,��ҪDebug
    };

    public abstract class BehaviorTreeNode : IDisposable
    {
        protected BehaviorTree Tree { get; private set; }
        protected BehaviorTreeExec Exec { get; private set; }
        private Action<NodeState> onStateChanged;

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

            onStateChanged?.Invoke(state);

            return state;
        }

        protected abstract NodeState DoExecute();

        public virtual void Dispose()
        {
            onStateChanged = null;
        }
    }
}