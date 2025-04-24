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

        private BehaviorTreeExec treeExec;

        private void Awake() => this.treeExec ??= new(this);

        private void Start() => this.treeExec.Boot();

        private void Update() => this.treeExec.Tick();

        private void OnDestroy()
        {
            treeExec.Dispose();
            treeExec = null;
        }
    }
}