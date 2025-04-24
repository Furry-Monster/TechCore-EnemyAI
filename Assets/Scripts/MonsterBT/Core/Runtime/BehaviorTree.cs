using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{

    /// <summary>
    /// 行为树数据结构
    /// </summary>
    public class BehaviorTree : IEnumerable<BehaviorTreeNode>, IDisposable
    {
        private Enter enter;

        public Enter Enter
        {
            get => enter;
            set => enter = value;
        }

        public BehaviorTree()
        {
            enter = BehaviorTreeData.BuildMockTree() as Enter;
            enter ??= new();
        }

        public BehaviorTree(BehaviorTreeData data)
        {
            enter = data.BuildTree() as Enter;
            enter ??= new();
        }

        public void LoadTreeData(BehaviorTreeData data)
        {
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