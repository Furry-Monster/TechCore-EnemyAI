using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{
    public enum MessageType
    {
        None,
        NodeStateChanged,  // 节点状态改变
        BlackboardChanged, // 黑板变量改变
        TreeExecutionEvent, // 树执行事件（启动、暂停、恢复、停止）
        Custom,            // 自定义消息
    }

    public struct BTMessage
    {
        public MessageType Type;
        public Guid SenderGUID;
        public object Data;
        public string CustomType;

        public BTMessage(MessageType type, Guid senderGUID, object data = null, string customType = null)
        {
            Type = type;
            SenderGUID = senderGUID;
            Data = data;
            CustomType = customType;
        }
    }

    /// <summary>
    /// 消息处理委托
    /// </summary>
    /// <param name="message">接收到的消息</param>
    public delegate void MessageHandler(BTMessage message);

    /// <summary>
    /// 行为树消息总线
    /// </summary>
    public class BehaviorTreeBus
    {
        private static BehaviorTreeBus instance;
        public static BehaviorTreeBus Instance
        {
            get
            {
                if (instance == null)
                    instance = new BehaviorTreeBus();
                return instance;
            }
        }

        private Dictionary<MessageType, List<MessageHandler>> subscribers;
        private Dictionary<string, List<MessageHandler>> customSubscribers;

        // 记录历史消息（用于调试）
        private Queue<BTMessage> messageHistory;
        private const int MAX_HISTORY_SIZE = 100;

        private BehaviorTreeBus()
        {
            subscribers = new Dictionary<MessageType, List<MessageHandler>>();
            customSubscribers = new Dictionary<string, List<MessageHandler>>();
            messageHistory = new Queue<BTMessage>();
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="handler">处理函数</param>
        public void Subscribe(MessageType type, MessageHandler handler)
        {
            if (!subscribers.ContainsKey(type))
            {
                subscribers[type] = new List<MessageHandler>();
            }
            subscribers[type].Add(handler);
        }

        /// <summary>
        /// 订阅自定义消息
        /// </summary>
        /// <param name="customType">自定义消息类型</param>
        /// <param name="handler">处理函数</param>
        public void SubscribeCustom(string customType, MessageHandler handler)
        {
            if (string.IsNullOrEmpty(customType))
            {
                Debug.LogError("[MonsterBT] Cannot subscribe to empty custom message type");
                return;
            }

            if (!customSubscribers.ContainsKey(customType))
            {
                customSubscribers[customType] = new List<MessageHandler>();
            }
            customSubscribers[customType].Add(handler);
        }

        /// <summary>
        /// 取消订阅消息
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="handler">处理函数</param>
        public void Unsubscribe(MessageType type, MessageHandler handler)
        {
            if (subscribers.ContainsKey(type))
            {
                subscribers[type].Remove(handler);
            }
        }

        /// <summary>
        /// 取消订阅自定义消息
        /// </summary>
        /// <param name="customType">自定义消息类型</param>
        /// <param name="handler">处理函数</param>
        public void UnsubscribeCustom(string customType, MessageHandler handler)
        {
            if (customSubscribers.ContainsKey(customType))
            {
                customSubscribers[customType].Remove(handler);
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message">消息内容</param>
        public void Publish(BTMessage message)
        {
            AddToHistory(message);

            if (subscribers.ContainsKey(message.Type))
            {
                // 创建副本以防在处理过程中有人取消订阅
                var handlers = new List<MessageHandler>(subscribers[message.Type]);
                foreach (var handler in handlers)
                {
                    try
                    {
                        handler(message);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[MonsterBT] Error handling message: {e.Message}");
                    }
                }
            }

            if (message.Type == MessageType.Custom && !string.IsNullOrEmpty(message.CustomType))
            {
                if (customSubscribers.ContainsKey(message.CustomType))
                {
                    // 创建副本以防在处理过程中有人取消订阅
                    var handlers = new List<MessageHandler>(customSubscribers[message.CustomType]);
                    foreach (var handler in handlers)
                    {
                        try
                        {
                            handler(message);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[MonsterBT] Error handling custom message: {e.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 发布节点状态改变消息
        /// </summary>
        /// <param name="nodeGUID">节点GUID</param>
        /// <param name="state">节点状态</param>
        public void PublishNodeStateChanged(Guid nodeGUID, NodeState state)
        {
            Publish(new BTMessage(MessageType.NodeStateChanged, nodeGUID, state));
        }

        /// <summary>
        /// 发布黑板变量改变消息
        /// </summary>
        /// <param name="variableName">变量名</param>
        /// <param name="newValue">新值</param>
        public void PublishBlackboardChanged(string variableName, object newValue)
        {
            var data = new Dictionary<string, object>
            {
                { "name", variableName },
                { "value", newValue }
            };
            Publish(new BTMessage(MessageType.BlackboardChanged, Guid.Empty, data));
        }

        /// <summary>
        /// 发布树执行事件消息
        /// </summary>
        /// <param name="eventName">事件名称 (start, pause, resume, stop)</param>
        public void PublishTreeExecutionEvent(string eventName)
        {
            Publish(new BTMessage(MessageType.TreeExecutionEvent, Guid.Empty, eventName));
        }

        /// <summary>
        /// 清除所有订阅
        /// </summary>
        public void ClearAllSubscriptions()
        {
            subscribers.Clear();
            customSubscribers.Clear();
        }

        /// <summary>
        /// 添加消息到历史记录
        /// </summary>
        private void AddToHistory(BTMessage message)
        {
            messageHistory.Enqueue(message);
            if (messageHistory.Count > MAX_HISTORY_SIZE)
            {
                messageHistory.Dequeue();
            }
        }

        /// <summary>
        /// 获取消息历史记录
        /// </summary>
        public BTMessage[] GetMessageHistory()
        {
            return messageHistory.ToArray();
        }

        /// <summary>
        /// 清除消息历史记录
        /// </summary>
        public void ClearMessageHistory()
        {
            messageHistory.Clear();
        }
    }
}