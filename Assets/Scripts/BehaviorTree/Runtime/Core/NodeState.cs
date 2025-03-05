namespace TechCore.BehaviorTree
{
    public enum NodeState
    {
        None,
        Ready,
        Running, // for Action nodes
        Success,
        Failure
    }
}