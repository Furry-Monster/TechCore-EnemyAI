using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterBT
{

    /// <summary>
    /// ��Ϊ�����ݽṹ
    /// </summary>
    public class BehaviorTree : IEnumerable<BehaviorTreeNode>, IDisposable
    {
        private Enter enter;
        public Enter Enter => enter;

        private List<Variable> treeVariables;
        public List<Variable> TreeVariables => treeVariables;

        private Blackboard blackboard;
        public Blackboard Blackboard => blackboard;

        public BehaviorTree()
        {
            var data = BehaviorTreeData.GenerateMockData();

            enter = data[0] as Enter;
            treeVariables = data[1] as List<Variable>;
            blackboard = data[2] as Blackboard;
            enter ??= new();
        }

        public BehaviorTree(BehaviorTreeData data)
        {
            enter = data.Tree as Enter;
            treeVariables = data.Variables.ToList();
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