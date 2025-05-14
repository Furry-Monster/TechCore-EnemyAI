using UnityEngine;

namespace MonsterBT
{
    public abstract class Conditional : BehaviorTreeNode
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
            OnStateChanged -= OnStateChanged += state =>
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
