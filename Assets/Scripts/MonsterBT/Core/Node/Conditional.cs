using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{
    public abstract class Conditional : BehaviorTreeNode
    {
        private BehaviorTreeNode parent;
        private BehaviorTreeNode child;

        protected override NodeState DoExecute()
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            base.Dispose();

            child.Dispose();
        }
    }
}
