using System.Collections.Generic;
using UnityEngine;

namespace TechCore.BehaviorTree
{
    public class BehaviorTreeMgr : MonoBehaviour
    {
        [SerializeField]
        private List<BehaviorTree> _behaviorTrees = new();

        private int _updateIndex = 0;
        private const int UPDATE_FREQUENCY = 5; // 每帧更新 5 个树

        private void Update()
        {
            var endIndex = Mathf.Min(_updateIndex + UPDATE_FREQUENCY, _behaviorTrees.Count);

            for (var i = _updateIndex; i < endIndex; i++)
            {
                _behaviorTrees[i].TickTree();
            }

            _updateIndex = (_updateIndex + UPDATE_FREQUENCY) % _behaviorTrees.Count;
        }

        public void AddBehaviorTree(BehaviorTree behaviorTree)
        {
            _behaviorTrees.Add(behaviorTree);
        }

        public void RemoveBehaviorTree(BehaviorTree behaviorTree)
        {
            _behaviorTrees.Remove(behaviorTree);
        }
    }
}