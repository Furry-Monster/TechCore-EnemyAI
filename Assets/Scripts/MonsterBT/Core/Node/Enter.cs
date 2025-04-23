using System;
using UnityEngine;

namespace MonsterBT
{
    public class Enter : BehaviorTreeNode
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
    }
}