using UnityEngine;

namespace MonsterBT.Runtime
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        [SerializeField] private BehaviorTree behaviorTreeAsset;
        [SerializeField] private bool runOnStart = true;
        [SerializeField] private bool debugMode = false;

        private BehaviorTree runtimeTree;
        private bool isRunning = false;

        public BehaviorTree RuntimeTree => runtimeTree;
        public bool IsRunning => isRunning;

        private void Start()
        {
            if (runOnStart)
            {
                StartTree();
            }
        }

        private void Update()
        {
            if (isRunning && runtimeTree != null)
            {
                var state = runtimeTree.Update();

                if (debugMode)
                {
                    Debug.Log($"[BehaviorTreeRunner] Tree state: {state}");
                }

                // 可以根据需要处理树的完成状态
                if (state == BTNodeState.Success || state == BTNodeState.Failure)
                {
                    // 树执行完成，可以选择重新开始或停止
                    // 这里选择重新开始（循环执行）
                    // StopTree();
                }
            }
        }

        public void StartTree()
        {
            if (behaviorTreeAsset != null)
            {
                runtimeTree = behaviorTreeAsset.Clone();
                runtimeTree.Initialize();

                // 设置一些默认的黑板值
                if (runtimeTree.Blackboard != null)
                {
                    runtimeTree.Blackboard.SetGameObject("Owner", gameObject);
                    runtimeTree.Blackboard.SetTransform("OwnerTransform", transform);
                }

                isRunning = true;

                if (debugMode)
                {
                    Debug.Log($"[BehaviorTreeRunner] Started behavior tree: {behaviorTreeAsset.name}");
                }
            }
        }

        public void StopTree()
        {
            if (runtimeTree != null)
            {
                runtimeTree.Abort();
                isRunning = false;

                if (debugMode)
                {
                    Debug.Log("[BehaviorTreeRunner] Stopped behavior tree");
                }
            }
        }

        public void RestartTree()
        {
            StopTree();
            StartTree();
        }

        private void OnDisable()
        {
            StopTree();
        }
    }
}