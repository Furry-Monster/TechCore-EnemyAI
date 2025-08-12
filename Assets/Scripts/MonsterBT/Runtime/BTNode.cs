using UnityEngine;

namespace MonsterBT.Runtime
{
    /// <summary>
    /// 行为树基类
    /// </summary>
    public abstract class BTNode : ScriptableObject
    {
        [SerializeField] protected string nodeName = "Node";
        [SerializeField] protected Vector2 position;

        public string NodeName => nodeName;

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        protected BTNodeState state = BTNodeState.Running;
        protected bool started = false;
        protected Blackboard blackboard;

        public BTNodeState State => state;

        public virtual void Initialize(Blackboard blackboard)
        {
            this.blackboard = blackboard;
        }

        public BTNodeState Update()
        {
            if (!started)
            {
                OnStart();
                started = true;
            }

            state = OnUpdate();

            if (state == BTNodeState.Success || state == BTNodeState.Failure)
            {
                OnStop();
                started = false;
            }

            return state;
        }

        public virtual BTNode Clone()
        {
            return GameObject.Instantiate(this);
        }

        public void Abort()
        {
            OnStop();
            started = false;
            state = BTNodeState.Failure;
        }

        protected virtual void OnStart()
        {
        }

        protected abstract BTNodeState OnUpdate();

        protected virtual void OnStop()
        {
        }
    }
}