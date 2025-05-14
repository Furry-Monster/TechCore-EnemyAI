using UnityEngine;

namespace MonsterBT
{
    public abstract class Conditional : BehaviorTreeNode, IHasSingleChild
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

        public virtual BehaviorTreeNode GetChild()
        {
            return child;
        }

        public int GetChildrenCount() => child == null ? 0 : 1;

        public virtual void SetChild(BehaviorTreeNode node)
        {
            child = node;
        }
    }
}
