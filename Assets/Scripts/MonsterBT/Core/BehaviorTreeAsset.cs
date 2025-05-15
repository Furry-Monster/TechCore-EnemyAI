using UnityEngine;

namespace MonsterBT
{
    [CreateAssetMenu(fileName = "BehaviorTreeAsset", menuName = "MonsterBT/BehaviorTreeAsset")]
    public class BehaviorTreeAsset : ScriptableObject
    {
        [SerializeField,Multiline]
        private string description;


    }
}