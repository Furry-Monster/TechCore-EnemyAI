using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{

    public abstract class BehaviorTreeNode : IDisposable
    {
        public virtual void Dispose()
        {

        }
    }

    public class Root : BehaviorTreeNode
    {
        private BehaviorTreeNode child;

        public override void Dispose()
        {
            child.Dispose();

            base.Dispose();
        }
    }

    public abstract class Action : BehaviorTreeNode
    {

    }

    public abstract class Composite : BehaviorTreeNode
    {
        private List<BehaviorTreeNode> children;

        public override void Dispose()
        {
            children.ForEach(e => e.Dispose());

            base.Dispose();
        }
    }

    public abstract class Decorator : BehaviorTreeNode
    {
        private BehaviorTreeNode child;

        public override void Dispose()
        {
            child.Dispose();

            base.Dispose();
        }
    }
}