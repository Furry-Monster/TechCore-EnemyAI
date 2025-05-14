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
            if (enter == null)
            {
                Debug.LogWarning("[MonsterBT] Cannot iterate over an empty tree");
                yield break;
            }

            // 使用Stack进行深度优先遍历
            Stack<BehaviorTreeNode> nodeStack = new Stack<BehaviorTreeNode>();
            nodeStack.Push(enter);

            while (nodeStack.Count > 0)
            {
                BehaviorTreeNode current = nodeStack.Pop();

                yield return current;

                if (current is IHasChildren multiChildNode)
                {
                    BehaviorTreeNode[] children = multiChildNode.GetChildren();
                    for (int i = children.Length - 1; i >= 0; i--)
                    {
                        if (children[i] != null)
                            nodeStack.Push(children[i]);
                    }
                }
                else if (current is IHasSingleChild singleChildNode)
                {
                    BehaviorTreeNode child = singleChildNode.GetChild();
                    if (child != null)
                        nodeStack.Push(child);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public BehaviorTreeNode FindNodeByGUID(Guid guid)
        {
            foreach (var node in this)
            {
                if (node.GUID == guid)
                    return node;
            }
            return null;
        }

        public void Dispose()
        {
            foreach (var node in this)
            {
                node.Dispose();
            }
            blackboard.Dispose();
        }
    }
}