using UnityEngine;

namespace MonsterBT
{
    /// <summary>
    /// ��Ϊ�����
    /// ���ض����Ƽ��Ǹ���AI����AI��ʹ��FSM��������ܡ�
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