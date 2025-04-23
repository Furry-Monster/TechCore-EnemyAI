using System;
using UnityEngine;

namespace MonsterBT
{
    public abstract class Composite : BehaviorTreeNode
    {
        private BehaviorTreeNode[] children;

        protected override void OnInitialize()
        {
            OnStateChanged += state =>
            {
                if (state == NodeState.Error)
                {
                    Debug.LogError($"[MonsterBT] {this.GetType()} occurred Error...");
                }
            };

            foreach (var child in children)
            {

                child?.Initalize(Tree, Exec);
            }
        }

        protected override NodeState DoExecute()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var child in children)
            {
                child.Dispose();
            }
        }
    }
}