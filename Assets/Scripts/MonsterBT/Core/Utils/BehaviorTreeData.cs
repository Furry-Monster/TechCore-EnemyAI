using System;
using System.Collections.Generic;

namespace MonsterBT
{
    public class BehaviorTreeData
    {
        public Variable[] Variables;

        public Blackboard Blackboard;

        /// <summary>
        /// For test
        /// </summary>
        public static List<Object> GenerateMockData()
        {
            List<Object> res = new();
            BehaviorTreeBuilder builder = new();

            // mock tree
            BehaviorTreeNode mockTree = builder
                .Action()
                .GetTree();
            res.Add(mockTree);

            // mock variables
            List<Variable> mockVariables = new();
            res.Add(mockVariables);

            // mock blackboard
            Blackboard mockBlackboard = new();
            res.Add(mockBlackboard);

            return res;
        }

        public BehaviorTreeNode BuildTree()
        {
            throw new NotImplementedException();
        }
    }
}