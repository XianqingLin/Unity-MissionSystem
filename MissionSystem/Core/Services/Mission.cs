using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Gameplay.MissionSystem
{
    public class Mission<T>
    {
        private readonly MissionPrototype<T> proto;
        private readonly List<MissionRequireHandle<T>> _unfinishedHandles = new();

        public MissionProperty Property => proto.property;
        public string Id => proto.id;
        public Mission(MissionPrototype<T> proto)
        {
            this.proto = proto;
            _unfinishedHandles = proto.requires.Select(r => r.CreateHandle()).ToList();
        }

        public void ApplyReward() => proto.ApplyReward();

        /// <summary>向任务发送玩家行为消息并检查任务是否完成以及是否发生状态变化</summary>
        /// <param name="message">玩家行为消息</param>
        /// <param name="hasStatusChanged">任务是否产生状态变化</param>
        /// <returns>任务是否完成</returns>
        public bool SendMessage(T message, out bool hasStatusChanged) =>
            proto.isSingleRequire
                ? _SendMessage_SingleRequire(message, out hasStatusChanged)
                : _SendMessage_MultiRequire(message, out hasStatusChanged);

        /// <summary>单需求的任务处理</summary>
        /// <param name="message"></param>
        /// <param name="hasStatusChanged"></param>
        /// <returns></returns>
        private bool _SendMessage_SingleRequire(T message, out bool hasStatusChanged) =>
            _unfinishedHandles[0].SendMessage(message, out hasStatusChanged);

        /// <summary>多需求任务处理</summary>
        /// <param name="message"></param>
        /// <param name="hasStatusChanged"></param>
        /// <returns></returns>
        private bool _SendMessage_MultiRequire(T message, out bool hasStatusChanged)
        {
            hasStatusChanged = false;
            var queueToRemove = new Queue<MissionRequireHandle<T>>();

            /* update all require handles */
            foreach (var requireHandle in _unfinishedHandles)
            {
                if (!requireHandle.SendMessage(message, out var _hasStatusChanged))
                {
                    hasStatusChanged |= _hasStatusChanged;
                    continue;
                }
                hasStatusChanged = true;
                if (proto.requireMode == MissionRequireMode.Any) return true;
                queueToRemove.Enqueue(requireHandle);
            }

            /* remove completed requries */
            while (queueToRemove.Count > 0)
            {
                var handle = queueToRemove.Dequeue();
                _unfinishedHandles.Remove(handle);
            }

            /* check if all requires have been completed */
            return _unfinishedHandles.Count == 0;
        }

#if UNITY_EDITOR
        public MissionPrototype<T> Proto => proto;

        // 给 Inspector 看还剩哪些需求未完成
        public IReadOnlyList<MissionRequireHandle<T>> EditorUnfinishedHandles => _unfinishedHandles;
#endif
    }
}
