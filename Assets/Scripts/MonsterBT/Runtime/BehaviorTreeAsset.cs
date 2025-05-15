using System;
using UnityEngine;

namespace MonsterBT
{
    [CreateAssetMenu(fileName = "BehaviorTreeAsset", menuName = "MonsterBT/BehaviorTreeAsset")]
    public class BehaviorTreeAsset : ScriptableObject
    {
        [SerializeField, Multiline]
        private string description;

        private BehaviorTreeData data;

        public BehaviorTreeData GetData()
        {
            return data;
        }

        public void SetData(BehaviorTreeData data)
        {
            this.data = data;
        }
    }

    public class BehaviorTreeData
    {
        public BehaviorTree Build()
        {
            throw new NotImplementedException();
        }
    }
}