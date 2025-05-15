using System;

namespace MonsterBT
{
    public class BehaviorTreeExecutor : IDisposable
    {
        public BehaviorTree Tree { get; set; }

        public Blackboard Blackboard { get; set; }

        public bool IsRunning = false;
        public bool IsRestarting = false;

        private Action<NodeState> OnRootStateNotify;

        public BehaviorTreeExecutor(BehaviorTree extTree = null, Blackboard extBlackboard = null)
        {
            Tree = extTree;
            Blackboard = extBlackboard;
        }

        /// <summary>
        /// ��ʼ����Ϊ��
        /// </summary>
        public void Initialize()
        {
            OnRootStateNotify += OnRootNotifySuccess;

            Tree.Root.Initialize();
        }

        private void OnRootNotifySuccess(NodeState rootState)
        {
            IsRunning = false;
            IsRestarting = true;
        }

        /// <summary>
        /// ������Ϊ��
        /// </summary>
        public void TreeBoot()
        {
            Tree.Root.Run();

            IsRunning = true;
        }

        /// <summary>
        /// ��Update֡������Ϊ������ȡÿһ��Update֡����״̬
        /// </summary>
        public void TreeTick()
        {
            if (IsRunning == false && IsRestarting)
            {
                TreeBoot();

                IsRestarting = false;
            }

            var rootState = Tree.Root.Tick();

            OnRootStateNotify?.Invoke(rootState);
        }

        /// <summary>
        /// �����Ϊ����Դ������
        /// </summary>
        public void Dispose()
        {
            Tree.Root.Dispose();

            OnRootStateNotify -= OnRootNotifySuccess;

            OnRootStateNotify = null;
        }
    }
}