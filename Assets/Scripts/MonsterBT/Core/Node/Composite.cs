using System;

namespace MonsterBT
{
    public abstract class Composite : BehaviorTreeNode
    {
        private BehaviorTreeNode parent;
        private BehaviorTreeNode[] children;

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