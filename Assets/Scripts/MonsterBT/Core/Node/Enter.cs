using UnityEngine;

namespace MonsterBT
{
    public class Enter : BehaviorTreeNode, IHasSingleChild
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

        protected override NodeState DoUpdate()
        {
            var state = child?.Update();

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
            return child;
        }

        public int GetChildrenCount() => child == null ? 0 : 1;

        public virtual void SetChild(BehaviorTreeNode node)
        {
            child = node;
        }
    }
}