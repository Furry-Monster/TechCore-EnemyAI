using System;
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
        public Guid GUID { get; private set; }

        private string nodeName;
        public string NodeName
        {
            get => string.IsNullOrEmpty(nodeName) ? GetType().Name : nodeName;
            set => nodeName = value;
        }

        public string Description { get; set; }

        private NodeState lastState = NodeState.Failure;
        public NodeState LastState => lastState;

        protected BehaviorTree Tree { get; private set; }
        protected BehaviorTreeExec Exec { get; private set; }
        protected GameObject GameObject { get; private set; }

        protected Action<NodeState> OnStateChanged;

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

        protected abstract void OnInitialize(); // 节点初始化逻辑

        public NodeState Execute()
        {
            try
            {
                var state = DoExecute();
                lastState = state;

                Debug.Log($"[MonsterBt] Node execution: {NodeName}, State: {state}");

                OnStateChanged?.Invoke(state);

                // 通过消息总线广播状态变化
                BehaviorTreeBus.Instance.PublishNodeStateChanged(GUID, state);

                return state;
            }
            catch (Exception e)
            {
                Debug.LogError($"[MonsterBT] Error executing node {NodeName}: {e.Message}");
                lastState = NodeState.Error;

                OnStateChanged?.Invoke(NodeState.Error);

                // 通过消息总线广播状态变化
                BehaviorTreeBus.Instance.PublishNodeStateChanged(GUID, NodeState.Error);

                return NodeState.Error;
            }
        }

        protected abstract NodeState DoExecute(); // 节点执行逻辑

        public virtual string GetDebugInfo()
        {
            return $"Node: {NodeName} ({GetType().Name}), Last State: {LastState}";
        }

        public virtual void Dispose()
        {
            OnStateChanged = null;
        }
    }

    #region interfaces
    public interface IHasChild
    {
        /// <summary>
        /// 获取子节点数目
        /// </summary>
        /// <returns>子节点数目</returns>
        public abstract int GetChildrenCount();
    }

    public interface IHasSingleChild : IHasChild
    {
        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <returns>节点实例</returns>
        public abstract BehaviorTreeNode GetChild();

        /// <summary>
        /// 设置子节点
        /// </summary>
        /// <param name="node"> 节点实例 </param>
        public abstract void SetChild(BehaviorTreeNode node);
    }

    public interface IHasChildren : IHasChild
    {
        /// <summary>
        /// 获取所有子节点
        /// </summary>
        /// <returns>节点数组</returns>
        public abstract BehaviorTreeNode[] GetChildren();

        /// <summary>
        /// 设置某一个子节点
        /// </summary>
        /// <param name="index">节点序号</param>
        /// <param name="node">节点实例</param>
        public abstract void SetChild(int index, BehaviorTreeNode node);
    }
    #endregion
}