namespace TechCore.BehaviorTree
{
    public class Repeater : BehaviorNode
    {
        private int _repeatCount;
        private int _currentCount;

        public Repeater(int count = -1)
        {
            _repeatCount = count;
        }

        public override void OnEnter()
        {
            _currentCount = 0;
        }

        public override NodeState OnUpdate()
        {
            if (Children.Count != 1)
                return NodeState.Failure;

            var childState = Children[0].OnUpdate();

            if (childState == NodeState.Running)
                return NodeState.Running;

            if (_repeatCount < 0) // 无限重复
                return NodeState.Running;

            _currentCount++;
            return _currentCount >= _repeatCount
                ? NodeState.Success
                : NodeState.Running;
        }
    }
}