using System.Collections;
using System.Collections.Generic;
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
        private BehaviorTreeSO externalBehaviorTree;

        [SerializeField]
        private BlackboardSO externalBlackboard;

        private BlackboardComp blackboardComp;

        private BehaviorTreeExec treeExec;

        private void Awake()
        {
            blackboardComp = this.GetComponent<BlackboardComp>();

            this.treeExec ??= new(this, blackboardComp);
        }

        private void Start() => this.treeExec.Boot();

        private void Update() => this.treeExec.Tick();

        private void OnDestroy()
        {
            treeExec.Dispose();
            treeExec = null;
        }
    }
}