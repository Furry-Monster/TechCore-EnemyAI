namespace TechCore.BehaviorTree
{
    public class Sequence : BehaviorNode
    {
        private int _currentChildIndex = 0;

        public override void OnEnter()
        {
            _currentChildIndex = 0;
        }

        public override NodeState OnUpdate()
        {
            if (Children.Count == 0)
                return NodeState.Success;

            var currentChild = Children[_currentChildIndex];
            var childState = currentChild.OnUpdate();

            switch (childState)
            {
                case NodeState.Running:
                    return NodeState.Running;

                case NodeState.Failure:
                    return NodeState.Failure;

                case NodeState.Success:
                    _currentChildIndex++;
                    return _currentChildIndex >= Children.Count
                        ? NodeState.Success
                        : NodeState.Running;

                default:
                    return NodeState.Failure;
            }
        }
    }
}