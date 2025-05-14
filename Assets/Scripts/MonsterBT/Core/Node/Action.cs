using UnityEngine;

namespace MonsterBT
{
    public abstract class Action : BehaviorTreeNode
    {

        protected override void OnInitialize()
        {
            OnStateChanged += state =>
            {
                if (state == NodeState.Error)
                {
                    Debug.LogError($"[MonsterBT] {this.GetType()} occurred Error...");
                }
            };
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

            base.Dispose();
        }
    }
}