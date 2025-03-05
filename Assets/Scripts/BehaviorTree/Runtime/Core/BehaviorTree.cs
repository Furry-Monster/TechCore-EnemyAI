using System.Collections.Generic;
using UnityEngine;

namespace TechCore.BehaviorTree
{
    public class BehaviorTree : MonoBehaviour
    {
        private bool _restartOnComplete = false;

        private BehaviorNode _rootNode;
        private BehaviorNode _currentNode; // optimization for update
        private bool _isRunning = false;
        private Dictionary<string, object> _blackboard = new();

        public void SetRoot(BehaviorNode root)
        {
            _rootNode = root;
            _rootNode.SetTree(this);
            InitializeTree(_rootNode);
        }
        private void InitializeTree(BehaviorNode node)
        {
            node.OnInit();

            foreach (var child in node.GetChildren())
            {
                child.SetTree(this);
                InitializeTree(child);
            }
        }

        public void StartTree()
        {
            if (_rootNode == null) return;
            _isRunning = true;
            _rootNode.OnEnter();
        }

        public void StopTree()
        {
            if (_rootNode == null) return;
            _isRunning = false;
            _rootNode.OnExit();
        }

        public void TickTree()
        {
            if (!_isRunning || _rootNode == null) return;
            _rootNode.OnUpdate();
        }

        #region Blackboard operations
        public void SetValue<T>(string key, T value)
        {
            _blackboard[key] = value;
        }

        public T GetValue<T>(string key)
        {
            if (_blackboard.TryGetValue(key, out object value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
            }
            return default;
        }

        public bool HasValue(string key)
        {
            return _blackboard.ContainsKey(key);
        }

        public void ClearValue(string key)
        {
            if (_blackboard.ContainsKey(key))
            {
                _blackboard.Remove(key);
            }
        }
        #endregion
    }
}