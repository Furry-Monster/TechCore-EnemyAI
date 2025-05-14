using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonsterBT
{
    public struct BehaviorTreeElem // struct for less memory usage
    {
        public BehaviorTreeNode node;
        public List<BTVariable> variables;

        public BehaviorTreeElem(BehaviorTreeNode node = null, List<BTVariable> variables = null)
        {
            this.node = node;
            this.variables = variables;
        }
    }

    public record BehaviorTreeData //record for transmitting ref
    {
        public BehaviorTreeNode Tree;

        public Blackboard Blackboard;

        // Create default tree data pack
        public BehaviorTreeData()
        {
            BehaviorTreeBuilder builder = new();

            // mock tree
            BehaviorTreeNode mockTree = builder
                .Action<Log>(new() {
                    { "Message", "If u see this, reflection has passed the test!" }
                })
                .GetTree();
            this.Tree = mockTree;

            // mock blackboard
            Blackboard mockBlackboard = new();
            this.Blackboard = mockBlackboard;
        }

        public BehaviorTreeData(BehaviorTreeNode root, Blackboard blackboard)
        {
            Tree = root;
            Blackboard = blackboard;
        }
    }

    public class BehaviorTreeBuilder
    {
        public Queue<BehaviorTreeElem> elemsQueue;

        public BehaviorTreeBuilder()
        {
            elemsQueue = new();
            BehaviorTreeElem root = new BehaviorTreeElem(new Enter());
            elemsQueue.Enqueue(root);
        }

        public BehaviorTreeBuilder Action<T>(Dictionary<string, object> initialValues = null) where T : Action, new()
        {
            BehaviorTreeElem action = new();
            BehaviorTreeNode actionNode = new T();
            action.node = actionNode;

            List<BTVariable> variablesToAdd = new();
            foreach (FieldInfo field in action.node.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                BTVariable variable = new ReflectionVariable(field, actionNode);
                // set initial values if exist
                if (initialValues != null && initialValues.TryGetValue(field.Name, out var value))
                {
                    variable.SetValue(value);
                }
                variablesToAdd.Add(variable);
            }
            action.variables = variablesToAdd;

            elemsQueue.Enqueue(action);
            return this;
        }

        public BehaviorTreeBuilder Build()
        {
            // here we build tree , as well as bind variables on blackboard
            Queue<BehaviorTreeElem> childCache = new();
            var root = elemsQueue.Dequeue();
            childCache.Enqueue(root);

            while (elemsQueue.Count > 0)
            {

            }

            throw new NotImplementedException();
        }

        public BehaviorTreeNode GetTree()
        {
            BehaviorTreeNode root = elemsQueue.Dequeue().node;
            BehaviorTreeNode iterator = root;

            while (elemsQueue.Count > 0)
            {
                var next = elemsQueue.Dequeue().node;
                if (iterator is IHasChildren hasChildren)
                {
                    hasChildren.SetChild(0, next);
                    iterator = next;
                }
            }

            return root;
        }

        public bool TryGetTree(out BehaviorTreeNode treeRoot)
        {
            treeRoot = elemsQueue.Dequeue().node;
            BehaviorTreeNode iterator = treeRoot;

            while (elemsQueue.Count > 0)
            {
                var next = elemsQueue.Dequeue().node;
                if (iterator is IHasChildren hasChildren)
                {
                    hasChildren.SetChild(0, next);
                    iterator = next;
                }
            }

            return true;
        }

        public Blackboard GetBlackboard()
        {
            throw new NotImplementedException();
        }

        public bool TryGetBlackboard(out Blackboard blackboard)
        {
            throw new NotImplementedException();
        }
    }
}