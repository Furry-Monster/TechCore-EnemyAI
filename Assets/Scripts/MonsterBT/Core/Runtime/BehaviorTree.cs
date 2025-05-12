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
        private Enter enter;
        public Enter Enter => enter;

        private List<BTVariable> treeVariables;
        public List<BTVariable> TreeVariables => treeVariables;

        private Blackboard blackboard;
        public Blackboard Blackboard => blackboard;

        public BehaviorTree()
        {
            var data = new BehaviorTreeData();

            enter = data.Tree as Enter;
            treeVariables = data.Variables;
            blackboard = data.Blackboard;
            enter ??= new();
        }

        public BehaviorTree(BehaviorTreeData data)
        {
            enter = data.Tree as Enter;
            treeVariables = data.Variables;
            blackboard = data.Blackboard;
            enter ??= new();
        }

        public void LoadTreeData(BehaviorTreeData data)
        {
            enter = data.Tree as Enter;
            treeVariables = data.Variables.ToList();
            blackboard = data.Blackboard;
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
            treeVariables.Clear();
            blackboard.Dispose();
        }
    }
}