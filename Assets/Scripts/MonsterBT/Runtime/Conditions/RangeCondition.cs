using UnityEngine;

namespace MonsterBT.Runtime.Conditions
{
    [CreateAssetMenu(fileName = "RangeCondition", menuName = "MonsterBTNode/Conditions/RangeCondition")]
    public class RangeCondition : ActionNode
    {
        [SerializeField] private string targetKey = "Target";
        [SerializeField] private float range = 5f;
        [SerializeField] private bool useSquareDistance = true; // 使用平方距离优化性能

        protected override BTNodeState OnUpdate()
        {
            var ownerTransform = blackboard.GetTransform("OwnerTransform");
            var targetObject = blackboard.GetGameObject(targetKey);

            if (ownerTransform == null || targetObject == null)
                return BTNodeState.Failure;

            bool inRange;

            if (useSquareDistance)
            {
                // 使用平方距离避免开方运算，提升性能
                float sqrDistance = (ownerTransform.position - targetObject.transform.position).sqrMagnitude;
                inRange = sqrDistance <= range * range;
            }
            else
            {
                float distance = Vector3.Distance(ownerTransform.position, targetObject.transform.position);
                inRange = distance <= range;
            }

            return inRange ? BTNodeState.Success : BTNodeState.Failure;
        }
    }
}