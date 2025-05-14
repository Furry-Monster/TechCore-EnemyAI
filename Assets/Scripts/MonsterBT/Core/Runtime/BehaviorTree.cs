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

        private Blackboard blackboard;
        public Blackboard Blackboard => blackboard;

        public BehaviorTree()
        {
            // create default tree data
            var data = new BehaviorTreeData();

            // bind tree structure
            enter = data.Tree as Enter;
            // bind data blackboard
            blackboard = data.Blackboard;
            enter ??= new();
        }

        public BehaviorTree(BehaviorTreeData data)
        {
            // bind tree structure
            enter = data.Tree as Enter;
            // bind data blackboard
            blackboard = data.Blackboard;
            enter ??= new();
        }

        public void LoadTreeData(BehaviorTreeData data)
        {
            // bind tree structure
            enter = data.Tree as Enter;
            // bind data blackboard
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
            blackboard.Dispose();
        }
    }
}