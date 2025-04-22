using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{
    /// <summary>
    /// 行为树组件
    /// 挂载对象推荐是复杂AI，简单AI请使用FSM以提高性能。
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