using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MonsterBT
{
    public class Enter : BehaviorTreeNode, IHasSingleChild
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

            child?.Initalize(Tree, Exec, GameObject);
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
            OnStateChanged -= state =>
            {
                if (state == NodeState.Error)
                {
                    Debug.LogError($"[MonsterBT] {this.GetType()} occurred Error...");
                }
            };

            child?.Dispose();

            base.Dispose();
        }

        public virtual BehaviorTreeNode GetChild()
        {
            throw new NotImplementedException();
        }

        public int GetChildrenCount() => child == null ? 0 : 1;

        public virtual void SetChild(BehaviorTreeNode node)
        {
            throw new NotImplementedException();
        }
    }
}