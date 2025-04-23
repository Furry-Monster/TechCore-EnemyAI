using System;

namespace MonsterBT
{
    public class BehaviorTreeData
    {
        public static BehaviorTreeNode BuildMockTree()
        {
            // mock data here
            return new Enter();
        }

        public BehaviorTreeNode BuildTree()
        {
            throw new NotImplementedException();
        }
    }
}