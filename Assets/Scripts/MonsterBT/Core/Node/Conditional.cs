using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterBT
{
    public abstract class Conditional : BehaviorTreeNode, IHasChildren
    {
        private BehaviorTreeNode child;

        protected override void OnInitialize()
        {
            OnStateChanged += state =>
            {
                if (state == NodeState.Error)
                {
                    Debug.LogError($"[MonsterBT] {this.GetType()} occurred Error...");
                }
            };

            child?.Initalize(Tree, Exec, GameObject);
        }

        public override void Dispose()
        {
            OnStateChanged -= OnStateChanged += state =>
            {
                if (state == NodeState.Error)
                {
                    Debug.LogError($"[MonsterBT] {this.GetType()} occurred Error...");
                }
            };

            child.Dispose();

            base.Dispose();
        }

        public virtual BehaviorTreeNode[] GetChildren() =>
            child == null
            ? Array.Empty<BehaviorTreeNode>()
            : new[] { child };

        public virtual int GetChildrenCount() => child == null ? 0 : 1;

        public virtual void SetChild(int index, BehaviorTreeNode node)
        {
            if (index > 0)
            {
                Debug.LogWarning(
                    $"[MonsterBT] The Node of Type {this.GetType()} has only 1 child\n" +
                    $"But we automatically set the node on index 0,instead of {index}");
            }
            else if (index < 0)
            {
                Debug.LogError($"[MonsterBT] Node index of {index} is InValid");
                return;
            }

            child = node;
        }

        public virtual void SetChildren(BehaviorTreeNode[] nodes)
        {
            if (nodes.Count() > 1)
            {
                Debug.LogWarning($"[MonsterBT] The Node of Type {this.GetType()} has only 1 child\n");
            }

            child = nodes[0];
        }
    }
}
