using UnityEngine;

namespace MonsterBT
{
    public abstract class Decorator : BehaviorTreeNode, IHasSingleChild
    {
        protected BehaviorTreeNode child;

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

        protected override void DoTick()
        {
            child?.Tick();
        }

        protected override NodeState DoUpdate()
        {
            var result = child.Update();
            result = DoDecorate(result);
            return result;
        }

        protected virtual NodeState DoDecorate(NodeState state)
        {
            return state;
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