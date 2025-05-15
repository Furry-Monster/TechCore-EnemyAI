using System;

namespace MonsterBT
{
    public enum NodeState
    {
        Success,
        Failure,
        Running,
    }

    public abstract class BehaviorTreeNode : IDisposable
    {
        public string Name;
        public Guid GUID;

        protected BehaviorTreeExecutor Executor;
        protected Action<NodeState> OnStateNotify;

        public void Initialize()
        {
            Name ??= this.GetType().ToString();

            GUID = Guid.NewGuid();

            OnInitialize();
        }

        protected abstract void OnInitialize();

        public void Start()
        {
            OnStart();
        }

        protected abstract void OnStart();

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