using UnityEngine;

namespace MonsterBT
{
    public class Sequence : Composite
    {
        private BehaviorTreeNode runningNode;

        public override bool CanUpdate()
        {
            foreach (var child in children)
            {
                if (!child.CanUpdate())
                    return false;
            }
            return true;
        }

        protected override NodeState DoUpdate()
        {
            foreach (var child in children)
            {
                var state = child.Update();

                switch (state)
                {
                    case NodeState.Success:
                        continue;
                }
            }

            return NodeState.Success;
        }
    }
}