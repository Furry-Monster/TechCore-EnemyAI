using UnityEngine;

namespace MonsterBT.Runtime.Conditions
{
    [CreateAssetMenu(fileName = "BlackboardBoolCondition",
        menuName = "MonsterBTNode/Conditions/BlackboardBoolCondition")]
    public class BlackboardBoolCondition : ActionNode
    {
        [SerializeField] private string keyName = "boolKey";
        [SerializeField] private bool expectedValue = true;

        protected override BTNodeState OnUpdate()
        {
            if (!blackboard.HasKey(keyName))
                return BTNodeState.Failure;

            bool currentValue = blackboard.GetBool(keyName);
            bool result = currentValue == expectedValue;

            return result ? BTNodeState.Success : BTNodeState.Failure;
        }
    }
}