using System;

namespace Gameplay.MissionSystem
{
    public enum MissionRequireMode
    {
        Any,
        All,
        Weight
    }

    /// <summary>决定玩家具体要执行的行为</summary>
    /// <typeparam name="T">消息类型</typeparam>
    [System.Serializable]
    public abstract class MissionRequire<T>
    {
        /// <summary>检查给定的消息是否对当前需求有效</summary>
        /// <param name="message">目标消息</param>
        /// <returns>是否有效</returns>
        public abstract bool CheckMessage(T message);

        /// <summary>创建当前需求的状态记录操作柄</summary>
        /// <returns></returns>
        public MissionRequireHandle<T> CreateHandle()
        {
            var _handleType = GetType().GetNestedType("Handle");
            if (_handleType == null)
                throw new Exception($"{GetType()} has not defined Handle");

            return (MissionRequireHandle<T>)Activator.CreateInstance(_handleType, this);
        }
    }
}
