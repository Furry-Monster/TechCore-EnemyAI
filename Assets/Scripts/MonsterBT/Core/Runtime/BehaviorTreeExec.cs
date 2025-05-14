using System;
using UnityEngine;

namespace MonsterBT
{
    /// <summary>
    /// 行为树执行器
    /// </summary>
    public class BehaviorTreeExec : IDisposable
    {
        private readonly BehaviorTree currentTree;
        public BehaviorTree CurrentTree => currentTree;

        private readonly BehaviorTreeComp BTComp;
        public BehaviorTreeComp BTComponent => BTComp;

        private readonly BlackboardComp BBComp;
        public BlackboardComp BBComponent => BBComp;

        private bool isRunning = false;
        public bool IsRunning => isRunning;

        private bool isPaused = false;
        public bool IsPaused => isPaused;

        public BehaviorTreeExec(BehaviorTreeComp bt, BlackboardComp bb)
        {
            currentTree ??= new();
            BTComp = bt;
            BBComp = bb;
        }

        /// <summary>
        /// 启动行为树执行
        /// </summary>
        public void Boot()
        {
            if (currentTree.Enter == null)
            {
                Debug.LogError("[MonsterBT] Failed to boot behavior tree: Enter node is null");
                return;
            }

            currentTree.Enter.Initalize(currentTree, this, BTComp.gameObject);
            isRunning = true;
            isPaused = false;

            Debug.Log("[MonsterBT] Behavior tree booted successfully");
        }

        /// <summary>
        /// 执行一次行为树的Tick
        /// </summary>
        public void Tick()
        {
            if (!isRunning || isPaused)
            {
                return;
            }

            if (currentTree.Enter == null)
            {
                Debug.LogWarning("[MonsterBT] Cannot tick behavior tree: Enter node is null");
                return;
            }

            NodeState state = currentTree.Enter.Execute();

            // TODO: 可以在这里添加对执行结果的处理
            if (state == NodeState.Error)
            {
                Debug.LogError("[MonsterBT] Behavior tree execution encountered an error");
                // 可以选择在错误时暂停树
                // isPaused = true;
            }
        }

        /// <summary>
        /// 暂停行为树执行
        /// </summary>
        public void Pause()
        {
            if (isRunning && !isPaused)
            {
                isPaused = true;
                Debug.Log("[MonsterBT] Behavior tree execution paused");
            }
        }

        /// <summary>
        /// 恢复行为树执行
        /// </summary>
        public void Resume()
        {
            if (isRunning && isPaused)
            {
                isPaused = false;
                Debug.Log("[MonsterBT] Behavior tree execution resumed");
            }
        }

        /// <summary>
        /// 完全停止行为树执行
        /// </summary>
        public void Halt()
        {
            isRunning = false;
            isPaused = false;

            // 这里可以添加清理逻辑，例如停止所有正在进行的行为
            // 可以添加通知或回调以便其他系统知道行为树已经停止

            Debug.Log("[MonsterBT] Behavior tree execution halted");
        }

        /// <summary>
        /// 重启行为树
        /// </summary>
        public void Restart()
        {
            Halt();
            Boot();
        }

        public void Dispose()
        {
            Halt();
            currentTree.Dispose();
        }
    }
}