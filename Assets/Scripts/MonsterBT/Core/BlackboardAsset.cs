using UnityEngine;

namespace MonsterBT
{
    [CreateAssetMenu(fileName = "BlackboardAsset", menuName = "MonsterBT/BlackboardAsset")]
    public class BlackboardAsset : ScriptableObject
    {
        [SerializeField, Multiline]
        private string description;
    }
}