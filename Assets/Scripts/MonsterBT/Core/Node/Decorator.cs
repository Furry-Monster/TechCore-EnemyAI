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

        protected override NodeState DoUpdate()
        {
            var result = child.Update();
            result = Decorate(result);
            return result;
        }

        protected virtual NodeState Decorate(NodeState state)
        {
            return state;
        }

        protected override void DoHalt()
        {
            child?.Halt();
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