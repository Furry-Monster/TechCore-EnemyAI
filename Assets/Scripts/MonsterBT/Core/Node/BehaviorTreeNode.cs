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

        public virtual bool CanUpdate() => true;

        public NodeState Update()
        {
            var state = DoUpdate();

            OnStateChanged?.Invoke(state);

            return state;
        }

        protected abstract NodeState DoUpdate(); // impl by nodes

        public void Halt()
        {
            Debug.Log($"[MonsterBt] Now halt node : {GetType()}");

            DoHalt();
        }

        protected abstract void DoHalt();   // impl by nodes

        public virtual void Dispose()
        {
            OnStateChanged = null;
        }
    }

    #region interfaces
    public interface IHasChild
    {
        public abstract int GetChildrenCount();
    }

    public interface IHasSingleChild : IHasChild
    {
        public abstract BehaviorTreeNode GetChild();

        public abstract void SetChild(BehaviorTreeNode node);
    }

    public interface IHasChildren : IHasChild
    {
        public abstract BehaviorTreeNode[] GetChildren();

        public abstract void SetChild(int index, BehaviorTreeNode node);
    }
    #endregion
}