using UnityEngine;

namespace MonsterBT
{

    public class BehaviorTreeComponent : MonoBehaviour
    {
        [Header("External Assets")]
        [SerializeField]
        private BehaviorTreeAsset externalTree;
        public BehaviorTreeAsset ExternalTree
        {
            get => externalTree;
            private set => externalTree = value;
        }

        [SerializeField]
        private BlackboardAsset externalBlackboard;
        public BlackboardAsset ExternalBlackboard
        {
            get => externalBlackboard;
            private set => externalBlackboard = value;
        }

        private BehaviorTreeExecutor executor;

        private void Awake()
        {
            executor = new BehaviorTreeExecutor();

            if (externalTree != null)
                executor.Tree = externalTree.GetData().Build();
            if (externalBlackboard != null)
                executor.Blackboard = externalBlackboard.GetData().Build();

            executor.Initialize();
        }

        private void Start()
        {
            executor.Start();
        }

        private void Update()
        {
            executor.TreeTick();
        }

        private void OnDestroy()
        {
            executor.Dispose();
        }
    }
}