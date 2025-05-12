using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonsterBT
{
    public struct BehaviorTreeElem // struct for less memory usage
    {
        public BehaviorTreeNode node;
        public List<Variable> variables;

        public BehaviorTreeElem(BehaviorTreeNode node = null, List<Variable> variables = null)
        {
            this.node = node ?? null;
            this.variables = variables;
        }
    }

    public record BehaviorTreeData //record for transmitting ref
    {
        public BehaviorTreeNode Tree;

        public List<Variable> Variables;

        public Blackboard Blackboard;

        // Create default tree data pack
        public BehaviorTreeData()
        {
            BehaviorTreeBuilder builder = new();

            // mock tree
            BehaviorTreeNode mockTree = builder
                .Action()
                .GetTree();
            this.Tree = mockTree;

            // mock variables
            List<Variable> mockVariables = new();
            this.Variables = mockVariables;

            // mock blackboard
            Blackboard mockBlackboard = new();
            this.Blackboard = mockBlackboard;
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

        public BehaviorTreeBuilder Action()
        {
            BehaviorTreeElem action = new();
            // default action node should be a logger  
            action.node = new Log();

            List<Variable> variables = new();
            foreach (var field in action.node.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                Variable variable = new ReflectionVariable(field);
                variables.Add(variable);
            }
            action.variables = variables;

            elemsQueue.Enqueue(action);
            return this;
        }

        public BehaviorTreeBuilder Action<T>() where T : Action, new()
        {
            BehaviorTreeElem action = new();
            action.node = new T();

            List<Variable> variables = new();
            foreach (var field in action.node.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                Variable variable = new ReflectionVariable(field);
                variables.Add(variable);
            }
            action.variables = variables;

            elemsQueue.Enqueue(action);
            return this;
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

        public Variable GetVariable()
        {
            throw new NotImplementedException();
        }

        public BehaviorTreeData Build()
        {
            throw new NotImplementedException();
        }

        public void Build(out BehaviorTreeData data)
        {
            throw new NotImplementedException();
        }
    }
}