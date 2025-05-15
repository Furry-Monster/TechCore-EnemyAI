using System.Collections;
using System.Collections.Generic;
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
        }

        private void Start()
        {
            executor.Initialize();
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