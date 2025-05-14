using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonsterBT
{
    public struct BehaviorTreeElem // struct for less memory usage
    {
        public BehaviorTreeNode node;
        public List<BTVariable> variables;
        public BehaviorTreeNode[] children;

        public BehaviorTreeElem(BehaviorTreeNode node = null, List<BTVariable> variables = null, BehaviorTreeNode[] children = null)
        {
            this.node = node;
            this.variables = variables;
            this.children = children;
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
            Blackboard mockBlackboard = builder.GetBlackboard();
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
        private bool isDirty = false;

        private BehaviorTreeNode root;
        private Blackboard blackboard;
        private Stack<BehaviorTreeNode> nodeStack;

        public Queue<BehaviorTreeElem> elemsQueue;

        public BehaviorTreeBuilder()
        {
            elemsQueue = new();
            nodeStack = new();

            BehaviorTreeElem firstElem = new(new Enter());
            elemsQueue.Enqueue(firstElem);

            isDirty = true;
        }

        public BehaviorTreeBuilder Action<T>(Dictionary<string, object> initialValues = null) where T : Action, new()
        {
            BehaviorTreeElem action = new();
            BehaviorTreeNode actionNode = new T();
            action.node = actionNode;

            // get all fields by reflection
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
            isDirty = true;

            return this;
        }

        public BehaviorTreeBuilder Composite<T>(params Func<BehaviorTreeBuilder, BehaviorTreeBuilder>[] childBuilders) where T : Composite, new()
        {
            var compositeNode = new T();
            List<BehaviorTreeNode> childNodes = new();

            // 保存当前队列状态
            var currentQueue = new Queue<BehaviorTreeElem>(elemsQueue);
            elemsQueue.Clear();

            foreach (var builder in childBuilders)
            {
                // 使用新的构建器来构建子树
                var childBuilder = new BehaviorTreeBuilder();
                builder(childBuilder);
                childBuilder.Build();

                if (childBuilder.root != null)
                    childNodes.Add(childBuilder.root);
            }

            elemsQueue = currentQueue;

            BehaviorTreeElem composite = new(compositeNode, null, childNodes.ToArray());
            elemsQueue.Enqueue(composite);
            isDirty = true;

            return this;
        }

        public BehaviorTreeBuilder Decorator<T>(Func<BehaviorTreeBuilder, BehaviorTreeBuilder> childBuilder) where T : Decorator, new()
        {
            var decoratorNode = new T();

            // 保存当前队列状态
            var currentQueue = new Queue<BehaviorTreeElem>(elemsQueue);
            elemsQueue.Clear();

            var builder = new BehaviorTreeBuilder();
            childBuilder(builder);
            builder.Build();

            elemsQueue = currentQueue;

            BehaviorTreeElem decorator = new(decoratorNode, null, new[] { builder.root });
            elemsQueue.Enqueue(decorator);
            isDirty = true;

            return this;
        }

        public BehaviorTreeBuilder Conditional<T>(Dictionary<string, object> initialValues = null) where T : Conditional, new()
        {
            BehaviorTreeElem conditional = new();
            BehaviorTreeNode conditionalNode = new T();
            conditional.node = conditionalNode;

            List<BTVariable> variablesToAdd = new();
            foreach (FieldInfo field in conditional.node.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                BTVariable variable = new ReflectionVariable(field, conditionalNode);
                // set initial values if exist
                if (initialValues != null && initialValues.TryGetValue(field.Name, out var value))
                {
                    variable.SetValue(value);
                }
                variablesToAdd.Add(variable);
            }
            conditional.variables = variablesToAdd;

            elemsQueue.Enqueue(conditional);
            isDirty = true;

            return this;
        }

        public BehaviorTreeBuilder Build()
        {
            if (!isDirty) return this;

            // here we build tree , as well as bind variables on blackboard
            BehaviorTreeNode root = elemsQueue.Dequeue().node;
            Blackboard blackboard = new();

            var current = root;

            while (elemsQueue.Count > 0)
            {
                var nextElem = elemsQueue.Dequeue();

                if (current is IHasSingleChild currentSingleChildNode)
                {
                    currentSingleChildNode.SetChild(nextElem.node);
                    current = nextElem.node;
                }
                else if (current is IHasChildren currentMultiChildNode && nextElem.children != null && nextElem.children.Length > 0)
                {
                    for (int i = 0; i < nextElem.children.Length; i++)
                    {
                        currentMultiChildNode.SetChild(i, nextElem.children[i]);
                    }
                }

                if (nextElem.variables != null)
                {
                    foreach (var variable in nextElem.variables)
                    {
                        blackboard.Add(variable);
                    }
                }
            }

            this.root = root;
            this.blackboard = blackboard;
            isDirty = false;

            return this;
        }

        public BehaviorTreeNode GetTree()
        {
            this.Build();

            return root;
        }

        public bool TryGetTree(out BehaviorTreeNode treeRoot)
        {
            this.Build();

            treeRoot = root;
            return treeRoot != null;
        }

        public Blackboard GetBlackboard()
        {
            this.Build();

            return blackboard;
        }

        public bool TryGetBlackboard(out Blackboard blackboard)
        {
            this.Build();

            blackboard = this.blackboard;
            return blackboard != null;
        }

        public BehaviorTreeData GetBehaviorTreeData()
        {
            this.Build();
            return new BehaviorTreeData(root, blackboard);
        }
    }
}