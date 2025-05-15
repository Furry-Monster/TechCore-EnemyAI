using UnityEngine;

namespace MonsterBT
{
    /// <summary>
    /// 行为树组件
    /// 挂载对象推荐是复杂AI，简单AI请使用FSM以提高性能。
    /// </summary>
    public class BehaviorTreeComp : MonoBehaviour
    {
        [SerializeField]
        private BehaviorTreeSO externalTree;

        [SerializeField]
        private BlackboardSO externalBlackboard;

        private BlackboardComp blackboardComp;

        private BehaviorTreeExec treeExec;

        private void Awake()
        {
            blackboardComp = this.GetComponent<BlackboardComp>();

            this.treeExec ??= new(this, blackboardComp);
        }

        private void Start() => this.treeExec.Initalize();

        private void Update() => this.treeExec.Update();

        private void OnDestroy()
        {
            treeExec.Dispose();
            treeExec = null;
        }
    }
}