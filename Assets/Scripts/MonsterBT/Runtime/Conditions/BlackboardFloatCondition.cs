using UnityEngine;

namespace MonsterBT.Runtime.Conditions
{
    [CreateAssetMenu(fileName = "BlackboardFloatCondition",
        menuName = "MonsterBTNode/Conditions/BlackboardFloatCondition")]
    public class BlackboardFloatCondition : ActionNode
    {
        [SerializeField] private string keyName = "floatKey";
        [SerializeField] private ComparisonType comparison = ComparisonType.Equal;
        [SerializeField] private float compareValue = 0f;
        [SerializeField] private float tolerance = 0.01f; // 精度

        protected override BTNodeState OnUpdate()
        {
            if (!blackboard.HasKey(keyName))
                return BTNodeState.Failure;

            float currentValue = blackboard.GetFloat(keyName);

            bool result = comparison switch
            {
                ComparisonType.LessThan => currentValue < compareValue,
                ComparisonType.LessThanOrEqual => currentValue <= compareValue,
                ComparisonType.Equal => Mathf.Abs(currentValue - compareValue) <= tolerance,
                ComparisonType.GreaterThanOrEqual => currentValue >= compareValue,
                ComparisonType.GreaterThan => currentValue > compareValue,
                _ => false
            };

            return result ? BTNodeState.Success : BTNodeState.Failure;
        }
    }
}