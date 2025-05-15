namespace MonsterBT
{
    public class Root : BehaviorTreeNode
    {
        private BehaviorTreeNode child;

        protected sealed override void OnInitialize()
        {
            child?.Initialize();
        }

        protected sealed override void OnBoot()
        {
            child?.Boot();
        }

        protected sealed override NodeState OnTick()
        {
            return child?.Tick() ?? NodeState.Failure;
        }

        public sealed override void Dispose()
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