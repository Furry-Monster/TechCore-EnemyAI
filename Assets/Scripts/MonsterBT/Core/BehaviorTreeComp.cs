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
        private BehaviorTreeExec treeExec;

        private void Awake() => this.treeExec ??= new();

        private void Start() => this.treeExec.Boot();

        private void Update() => this.treeExec.Tick();

        private void OnDestroy()
        {
            this.treeExec.Dispose();
            this.treeExec = null;
        }
    }
}