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

        public BehaviorTree() { }
        public BehaviorTree(BehaviorTreeData data)
        {
            throw new NotImplementedException();
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
        }
    }
}