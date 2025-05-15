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
        /// 初始化行为树
        /// </summary>
        public void Initialize()
        {
            // 如果根节点状态返回Success
            OnRootStateNotify += rootState =>
            {
                if (rootState == NodeState.Success)
                {
                    // 设置重新启动
                    IsRunning = false;
                    IsRestarting = true;
                }
            };

            Tree.Root.Initialize();
        }

        /// <summary>
        /// 启动行为树
        /// </summary>
        public void TreeBoot()
        {
            Tree.Root.Boot();

            IsRunning = true;
        }

        /// <summary>
        /// 按Update帧更新行为树，获取每一个Update帧的树状态
        /// </summary>
        public void TreeTick()
        {
            if (!IsRunning && IsRestarting)
            {
                TreeBoot();

                IsRestarting = false;
            }

            var rootState = Tree.Root.Tick();

            OnRootStateNotify?.Invoke(rootState);
        }

        /// <summary>
        /// 清除行为树资源与依赖
        /// </summary>
        public void Dispose()
        {
            Tree.Root.Dispose();

            OnRootStateNotify = null;
        }
    }
}