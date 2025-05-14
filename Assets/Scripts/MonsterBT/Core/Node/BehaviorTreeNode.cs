using System;
using UnityEngine;

namespace MonsterBT
{

    public enum NodeState
    {
        Executing,
        Success,
        Failure,
        Error,
    };

    public abstract class BehaviorTreeNode : IDisposable
    {
        public Guid GUID { get; private set; }

        protected BehaviorTree Tree { get; private set; }
        protected BehaviorTreeExec Exec { get; private set; }
        protected GameObject GameObject { get; private set; }
        protected Action<NodeState> OnStateChanged; // state change event

        public BehaviorTreeNode()
        {
            GUID = Guid.NewGuid();
        }

        public void Initalize(BehaviorTree tree, BehaviorTreeExec exec, GameObject gameObject)
        {
            Tree = tree;
            Exec = exec;
            GameObject = gameObject;

            OnInitialize();
        }

        protected abstract void OnInitialize(); // impl by nodes

        public virtual bool CanExecute() => true;

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

    #region interfaces
    public interface IHasChild
    {
        /// <summary>
        /// ��ȡ�ӽڵ���Ŀ
        /// </summary>
        /// <returns>�ӽڵ���Ŀ</returns>
        public abstract int GetChildrenCount();
    }

    public interface IHasSingleChild : IHasChild
    {
        /// <summary>
        /// ��ȡ�ӽڵ�
        /// </summary>
        /// <returns>�ڵ�ʵ��</returns>
        public abstract BehaviorTreeNode GetChild();

        /// <summary>
        /// �����ӽڵ�
        /// </summary>
        /// <param name="node"> �ڵ�ʵ�� </param>
        public abstract void SetChild(BehaviorTreeNode node);
    }

    public interface IHasChildren : IHasChild
    {
        /// <summary>
        /// ��ȡ�����ӽڵ�
        /// </summary>
        /// <returns>�ڵ�����</returns>
        public abstract BehaviorTreeNode[] GetChildren();

        /// <summary>
        /// ����ĳһ���ӽڵ�
        /// </summary>
        /// <param name="index">�ڵ���</param>
        /// <param name="node">�ڵ�ʵ��</param>
        public abstract void SetChild(int index, BehaviorTreeNode node);
    }
    #endregion
}