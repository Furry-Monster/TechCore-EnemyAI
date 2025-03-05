namespace TechCore.BehaviorTree
{
    public class Inverter : BehaviorNode
    {
        public override NodeState OnUpdate()
        {
            if (Children.Count != 1)
                return NodeState.Failure;

            var childState = Children[0].OnUpdate();

            return childState switch
            {
                NodeState.None => NodeState.None,
                NodeState.Ready => NodeState.Ready,
                NodeState.Running => NodeState.Running,
                NodeState.Success => NodeState.Failure,
                NodeState.Failure => NodeState.Success,
                _ => NodeState.Failure
            };
        }
    }
}