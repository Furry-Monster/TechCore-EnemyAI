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
        /// <param name="index">节点标号</param>
        /// <param name="node">节点实例</param>
        public abstract void SetChild(int index, BehaviorTreeNode node);
    }
    #endregion
}