using UnityEngine;

namespace MonsterBT.Runtime
{
    [CreateAssetMenu(fileName = "BehaviorTree", menuName = "MonsterBT/BehaviorTree")]
    public class BehaviorTree : ScriptableObject
    {
        [SerializeField] private RootNode rootNode;
        [SerializeField] private Blackboard blackboard;

        private BTNodeState treeState = BTNodeState.Running;

        public RootNode RootNode
        {
            get => rootNode;
            set => rootNode = value;
        }

        public Blackboard Blackboard
        {
            get => blackboard;
            set => blackboard = value;
        }

        public BTNodeState TreeState => treeState;

        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            tree.rootNode = rootNode?.Clone() as RootNode;
            tree.blackboard = Instantiate(blackboard);
            return tree;
        }

        public void Initialize()
        {
            if (blackboard == null)
            {
                blackboard = CreateInstance<Blackboard>();
            }

            rootNode?.Initialize(blackboard);
        }

        public BTNodeState Update()
        {
            if (rootNode != null)
            {
                treeState = rootNode.Update();
            }
            else
            {
                treeState = BTNodeState.Failure;
            }

            return treeState;
        }

        public void Abort()
        {
            rootNode?.Abort();
            treeState = BTNodeState.Failure;
        }
    }
}