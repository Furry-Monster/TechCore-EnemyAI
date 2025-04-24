using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterBT
{

    /// <summary>
    /// 行为树数据结构
    /// </summary>
    public class BehaviorTree : IEnumerable<BehaviorTreeNode>, IDisposable
    {
        private List<Variable> treeVariables;

        public List<Variable> TreeVariables => treeVariables;

        private Blackboard blackboard;

        public Blackboard Blackboard => blackboard;

        private Enter enter;

        public Enter Enter => enter;

        public BehaviorTree()
        {
            var data = BehaviorTreeData.GenerateMockData();
            treeVariables = data[1] as List<Variable>;
            blackboard = data[2] as Blackboard;
            enter = data[0] as Enter;
            enter ??= new();
        }

        public BehaviorTree(BehaviorTreeData data)
        {
            treeVariables = data.Variables.ToList();
            blackboard = data.Blackboard;
            enter = data.BuildTree() as Enter;
            enter ??= new();
        }

        public void LoadTreeData(BehaviorTreeData data)
        {
            treeVariables = data.Variables.ToList();
            blackboard = data.Blackboard;
            enter = data.BuildTree() as Enter;
            enter ??= new();
        }

        public IEnumerator<BehaviorTreeNode> GetEnumerator()
        {
            // implement iterator here...
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            enter.Dispose();
            enter = null;
        }
    }
}