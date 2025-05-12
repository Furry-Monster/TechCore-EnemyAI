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
        protected GameObject GameObject { get; private set; }
        protected Action<NodeState> OnStateChanged; // state change event

        public void Initalize(BehaviorTree tree, BehaviorTreeExec exec, GameObject gameObject)
        {
            Tree = tree;
            Exec = exec;
            GameObject = gameObject;

            OnInitialize();
        }

        protected abstract void OnInitialize(); // impl by nodes

        public NodeState Execute()
        {
            var state = DoExecute();

            Debug.Log($"[MonsterBt] Now exec node : {GetType()}");

            OnStateChanged?.Invoke(state);

            return state;
        }

        protected abstract NodeState DoExecute(); // impl by nodes

        public virtual void Dispose()
        {
            OnStateChanged = null;
        }
    }

    public interface IHasChildren
    {
        /// <summary>
        /// ��ȡ�ӽڵ�
        /// </summary>
        /// <returns></returns>
        public abstract BehaviorTreeNode[] GetChildren();

        /// <summary>
        /// ��ȡ�ӽڵ���Ŀ
        /// </summary>
        /// <returns></returns>
        public abstract int GetChildrenCount();

        /// <summary>
        /// ����ĳһ���ӽڵ�
        /// </summary>
        /// <param name="index">�ڵ���</param>
        /// <param name="node">�ڵ�ʵ��</param>
        public abstract void SetChild(int index, BehaviorTreeNode node);

        /// <summary>
        /// ���������ӽڵ�
        /// </summary>
        /// <param name="nodes">�ڵ�ʵ����</param>
        public abstract void SetChildren(BehaviorTreeNode[] nodes);
    }
}