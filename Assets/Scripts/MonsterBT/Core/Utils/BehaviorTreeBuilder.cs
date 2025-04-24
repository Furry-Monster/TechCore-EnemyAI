using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonsterBT
{
    public record BehaviorTreeElem
    {
        public BehaviorTreeNode node;
        public Variable[] variables;

        public BehaviorTreeElem(BehaviorTreeNode node = null, Variable[] variables = null)
        {
            this.node = node ?? null;
            this.variables = variables ?? Array.Empty<Variable>();
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
            action.variables = variables.ToArray();

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
            action.variables = variables.ToArray();

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

        public BehaviorTree Build()
        {
            throw new NotImplementedException();
        }

        public void Build(out BehaviorTree tree)
        {
            throw new NotImplementedException();
        }
    }
}