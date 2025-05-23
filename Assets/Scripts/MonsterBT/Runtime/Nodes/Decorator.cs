namespace MonsterBT
{
    public abstract class Decorator : BehaviorTreeNode
    {
        private BehaviorTreeNode child;

        protected override void OnInitialize()
        {
            child?.Initialize();
        }

        protected override void OnBoot()
        {
            child?.Boot();
        }

        protected override NodeState OnTick()
        {
            var state = child?.Tick() ?? NodeState.Failure;

            return OnDecorate(state);
        }

        protected virtual NodeState OnDecorate(NodeState stateToDecorate)
        {
            return stateToDecorate;
        }

        public override void Dispose()
        {
            child?.Dispose();

            base.Dispose();
        }

        public BehaviorTreeNode GetChild()
        {
            return child;
        }

        public void SetChild(BehaviorTreeNode nodeToSet)
        {
            child = nodeToSet;
        }
    }
}