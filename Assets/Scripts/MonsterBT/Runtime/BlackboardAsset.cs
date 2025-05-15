using System;
using UnityEngine;

namespace MonsterBT
{
    [CreateAssetMenu(fileName = "BlackboardAsset", menuName = "MonsterBT/BlackboardAsset")]
    public class BlackboardAsset : ScriptableObject
    {
        [SerializeField, Multiline]
        private string description;

        private BlackboardData data;

        public BlackboardData GetData()
        {
            return data;
        }

        public void SetData(BlackboardData data)
        {
            this.data = data;
        }
    }

    public class BlackboardData
    {
        public Blackboard Build()
        {
            throw new NotImplementedException();
        }
    }
}