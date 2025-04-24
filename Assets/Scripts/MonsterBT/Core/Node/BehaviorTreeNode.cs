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

    public interface IHasChildren
    {
        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <returns></returns>
        public abstract BehaviorTreeNode[] GetChildren();

        /// <summary>
        /// 获取子节点数目
        /// </summary>
        /// <returns></returns>
        public abstract int GetChildrenCount();

        /// <summary>
        /// 设置某一个子节点
        /// </summary>
        /// <param name="index">节点标号</param>
        /// <param name="node">节点实例</param>
        public abstract void SetChild(int index, BehaviorTreeNode node);

        /// <summary>
        /// 设置所有子节点
        /// </summary>
        /// <param name="nodes">节点实例组</param>
        public abstract void SetChildren(BehaviorTreeNode[] nodes);
    }
}