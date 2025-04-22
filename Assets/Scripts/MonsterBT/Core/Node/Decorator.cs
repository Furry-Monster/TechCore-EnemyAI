using System;

namespace MonsterBT
{
    public abstract class Decorator : BehaviorTreeNode
    {
        private BehaviorTreeNode parent;
        private BehaviorTreeNode child;

        protected override NodeState DoExecute()
        {
            var state = child.Execute();

            return state;
        }

        public override void Dispose()
        {
            base.Dispose();

            child.Dispose();
        }
    }
}