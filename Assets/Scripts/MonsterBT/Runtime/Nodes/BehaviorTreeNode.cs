using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MonsterBT
{
    public enum NodeState
    {
        Success,
        Failure,
        Running,
    }

    public abstract class BehaviorTreeNode : IDisposable
    {
        public string Name;
        public Rect graphPosition;
        public Guid GUID;
        public Action<NodeState> OnStateChanged;

        public void Initialize()
        {
            Name ??= this.GetType().ToString();

            if (graphPosition == default)
            {
                graphPosition = new Rect(400, 300, 100, 100);
            }

            GUID = Guid.NewGuid();

            OnInitialize();
        }

        protected abstract void OnInitialize();

        public void Start()
        {
            OnStart();
        }

        protected abstract void OnStart();

        public NodeState Tick()
        {
            var state = OnTick();

            OnStateChanged?.Invoke(state);

            return state;
        }

        protected abstract NodeState OnTick();

        public virtual void Dispose()
        {

        }
    }

    public class Root : BehaviorTreeNode
    {
        private BehaviorTreeNode child;

        protected sealed override void OnInitialize()
        {
            OnStateChanged += OnRootStateChanged;

            child?.Initialize();
        }

        private void OnRootStateChanged(NodeState tickState)
        {
            if (tickState == NodeState.Success)
            {
                // this means the whole tree finish 1 round running

            }
            else if (tickState == NodeState.Failure)
            {
                // this means some failure wasn't handled by node
            }
        }

        protected sealed override void OnStart()
        {
            child?.Start();
        }

        protected sealed override NodeState OnTick()
        {
            return child?.Tick() ?? NodeState.Failure;
        }

        public sealed override void Dispose()
        {
            child?.Dispose();

            base.Dispose();
        }

        public BehaviorTreeNode GetChild()
        {
            return child;
        }

        public void SetChild(BehaviorTreeNode nodeToSet)
        {
            child = nodeToSet;
        }
    }

    public abstract class Action : BehaviorTreeNode
    {

    }

    public abstract class Composite : BehaviorTreeNode
    {
        private List<BehaviorTreeNode> children;

        protected override void OnInitialize()
        {
            children?.ForEach(e => e.Initialize());
        }

        protected override void OnStart()
        {
            children?.ForEach(e => e.Start());
        }

        public override void Dispose()
        {
            children?.ForEach(e => e.Dispose());

            base.Dispose();
        }

        #region children ops
        public void AddChild(BehaviorTreeNode nodeToAdd)
        {
            children.Add(nodeToAdd);
        }

        public void SetChild(int index, BehaviorTreeNode nodeToSet)
        {
            children[index] = nodeToSet;
        }

        public void RemoveChild(int index)
        {
            children.RemoveAt(index);
        }

        public BehaviorTreeNode GetChild(int index)
        {
            return children[index];
        }

        public List<BehaviorTreeNode> GetChildren()
        {
            return children;
        }

        public int GetChildCount()
        {
            return children.Count;
        }
        #endregion
    }

    public abstract class Decorator : BehaviorTreeNode
    {
        private BehaviorTreeNode child;

        protected override void OnInitialize()
        {
            child?.Initialize();
        }

        protected override void OnStart()
        {
            child?.Start();
        }

        protected override NodeState OnTick()
        {
            var state = child?.Tick() ?? NodeState.Failure;

            return OnDecorate(state);
        }

        protected virtual NodeState OnDecorate(NodeState stateToDecorate)
        {
            return stateToDecorate;
        }

        public override void Dispose()
        {
            child?.Dispose();

            base.Dispose();
        }

        public BehaviorTreeNode GetChild()
        {
            return child;
        }

        public void SetChild(BehaviorTreeNode nodeToSet)
        {
            child = nodeToSet;
        }
    }
}