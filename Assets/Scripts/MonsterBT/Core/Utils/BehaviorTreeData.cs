using System;

namespace MonsterBT
{
    public class BehaviorTreeData
    {
        public static BehaviorTreeNode BuildMockTree()
        {
            // mock data here
            Enter root = new Enter();
            root.SetChild(0, new Log());
            return root;
        }

        public BehaviorTreeNode BuildTree()
        {
            throw new NotImplementedException();
        }
    }
}