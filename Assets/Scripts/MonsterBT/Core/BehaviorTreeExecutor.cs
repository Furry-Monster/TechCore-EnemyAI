using System;

namespace MonsterBT
{
    public class BehaviorTreeExecutor : IDisposable
    {
        public BehaviorTree Tree { get; set; }

        public Blackboard Blackboard { get; set; }

        public BehaviorTreeExecutor(BehaviorTree extTree = null, Blackboard extBlackboard = null)
        {
            Tree = extTree;
            Blackboard = extBlackboard;
        }

        public void Initialize()
        {

        }

        public void TreeTick()
        {

        }

        public void Dispose()
        {

        }
    }
}