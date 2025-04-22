using System;

namespace MonsterBT
{
    public abstract class Action : BehaviorTreeNode
    {
        private BehaviorTreeNode parent;

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}