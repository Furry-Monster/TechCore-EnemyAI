using System.Collections.Generic;

namespace TechCore.BehaviorTree
{
    public abstract class BehaviorNode
    {
        protected BehaviorTree Tree;
        protected BehaviorNode Parent;
        protected List<BehaviorNode> Children = new();

        public NodeState CurrentState { get; protected set; } = NodeState.None;

        #region Callbacks
        public virtual void OnInit() { }

        public virtual void OnEnter() { }

        public virtual NodeState OnUpdate()
        {
            return NodeState.Success;
        }

        public virtual void OnExit() { }
        #endregion

        public void SetTree(BehaviorTree tree)
        {
            this.Tree = tree;
        }

        public void AddChild(BehaviorNode child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void RemoveChild(BehaviorNode child)
        {
            if (Children.Contains(child))
            {
                child.Parent = null;
                Children.Remove(child);
            }
        }

        public List<BehaviorNode> GetChildren()
        {
            return Children ??= new List<BehaviorNode>();
        }
    }
}