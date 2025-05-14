using UnityEngine;

namespace MonsterBT
{
    public class Sequence : Composite
    {
        private BehaviorTreeNode runningNode;

        public override bool CanExecute()
        {
            foreach (var child in children)
            {
                if (!child.CanExecute())
                    return false;
            }
            return true;
        }

        protected override NodeState DoExecute()
        {
            foreach (var child in children)
            {
                var state = child.Execute();

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