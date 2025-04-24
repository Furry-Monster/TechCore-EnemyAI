using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MonsterBT
{
    public class Enter : BehaviorTreeNode, IHasChildren
    {
        private BehaviorTreeNode child;

        private BehaviorTreeNode Child
        {
            get => child;
            set => child = value;
        }

        protected override void OnInitialize()
        {
            OnStateChanged += state =>
            {
                if (state == NodeState.Error)
                {
                    Debug.LogError($"[MonsterBT] {this.GetType()} occurred Error...");
                }
            };

            child?.Initalize(Tree, Exec);
        }

        protected override NodeState DoExecute()
        {
            var state = child?.Execute();

            return state == null
                ? NodeState.Error
                : (NodeState)state;
        }

        public override void Dispose()
        {
            child?.Dispose();
            base.Dispose();
        }

        public BehaviorTreeNode[] GetChildren() =>
            child == null
            ? Array.Empty<BehaviorTreeNode>()
            : new[] { child };

        public int GetChildrenCount() => child == null ? 0 : 1;

        public void SetChild(int index, BehaviorTreeNode node)
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

        public void SetChildren(BehaviorTreeNode[] nodes)
        {
            if (nodes.Count() > 1)
            {
                Debug.LogWarning($"[MonsterBT] The Node of Type {this.GetType()} has only 1 child\n");
            }

            child = nodes[0];
        }
    }
}