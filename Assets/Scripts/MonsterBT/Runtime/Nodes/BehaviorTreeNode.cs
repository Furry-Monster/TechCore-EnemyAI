using System;

namespace MonsterBT
{
    public enum NodeState
    {
        Success,
        Failure,
        Running,
        None,
    }

    public abstract class BehaviorTreeNode : IDisposable
    {
        public string Name;
        public Guid GUID;

        protected BehaviorTreeExecutor Executor;
        protected NodeState CurrentState;
        protected Action<NodeState> OnStateNotify;

        public void Initialize()
        {
            Name ??= this.GetType().ToString();

            GUID = Guid.NewGuid();

            CurrentState = NodeState.None;

            OnInitialize();
        }

        protected abstract void OnInitialize();

        public void Boot()
        {
            CurrentState = NodeState.Running;

            OnBoot();
        }

        protected abstract void OnBoot();

        public NodeState Tick()
        {
            var state = OnTick();

            OnStateNotify?.Invoke(state);

            return state;
        }

        protected abstract NodeState OnTick();

        public virtual void Dispose()
        {
            OnStateNotify = null;
        }
    }
}