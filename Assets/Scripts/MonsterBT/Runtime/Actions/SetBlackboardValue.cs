using UnityEngine;
using MonsterBT.Runtime.Utils;

namespace MonsterBT.Runtime.Actions
{
    /// <summary>
    /// 设置黑板值节点：用于在黑板中设置指定键的值
    /// </summary>
    [CreateAssetMenu(fileName = "SetBlackboardValue", menuName = "MonsterBTNode/Actions/SetBlackboardValue")]
    public class SetBlackboardValue : ActionNode
    {
        [SerializeField] private string key;
        [SerializeField] private ValueType valueType = ValueType.String;

        [SerializeField] private string stringValue;
        [SerializeField] private float floatValue;
        [SerializeField] private bool boolValue;
        [SerializeField] private Vector3 vector3Value;
        [SerializeField] private GameObject gameObjectValue;

        private enum ValueType
        {
            String,
            Float,
            Bool,
            Vector3,
            GameObject
        }

        protected override BTNodeState OnUpdate()
        {
            if (string.IsNullOrEmpty(key) || blackboard == null)
                return BTNodeState.Failure;

            switch (valueType)
            {
                case ValueType.String:
                    blackboard.SetString(key, stringValue);
                    break;
                case ValueType.Float:
                    blackboard.SetFloat(key, floatValue);
                    break;
                case ValueType.Bool:
                    blackboard.SetBool(key, boolValue);
                    break;
                case ValueType.Vector3:
                    blackboard.SetVector3(key, vector3Value);
                    break;
                case ValueType.GameObject:
                    blackboard.SetGameObject(key, gameObjectValue);
                    break;
                default:
                    return BTNodeState.Failure;
            }

            return BTNodeState.Success;
        }
    }
}