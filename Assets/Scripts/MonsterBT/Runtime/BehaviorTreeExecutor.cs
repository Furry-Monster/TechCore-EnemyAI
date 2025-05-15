using System;

namespace MonsterBT
{
    public class BehaviorTreeExecutor : IDisposable
    {
        public BehaviorTree Tree { get; set; }

        public Blackboard Blackboard { get; set; }

        public bool IsRunning = false;

        private Action<NodeState> OnRootStateNotify;

        public BehaviorTreeExecutor(BehaviorTree extTree = null, Blackboard extBlackboard = null)
        {
            Tree = extTree;
            Blackboard = extBlackboard;
        }

        public void Initialize()
        {
            OnRootStateNotify += OnRootNotifySuccess;

            Tree.Root.Initialize();
        }

        private void OnRootNotifySuccess(NodeState rootState) => IsRunning = false;

        public void Start()
        {
            Tree.Root.Start();

            IsRunning = true;
        }

        public void TreeTick()
        {
            if (IsRunning == false)
            {
                Start();
            }

            var rootState = Tree.Root.Tick();

            OnRootStateNotify?.Invoke(rootState);
        }

        public void Dispose()
        {
            Tree.Root.Dispose();

            OnRootStateNotify -= OnRootNotifySuccess;

            OnRootStateNotify = null;
        }
    }
}