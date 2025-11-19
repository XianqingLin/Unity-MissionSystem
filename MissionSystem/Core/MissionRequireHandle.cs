
namespace Gameplay.MissionSystem
{
    /// <summary>记录玩家当前的某个任务需求状态</summary>
    public abstract class MissionRequireHandle<T>
    {
        private readonly MissionRequire<T> _require;

        protected MissionRequireHandle(MissionRequire<T> require)
        {
            _require = require;
        }

        /// <summary>发送一条消息给玩家</summary>
        /// <param name="message"></param>
        /// <param name="hasStatusChanged"></param>
        /// <returns></returns>
        public bool SendMessage(T message, out bool hasStatusChanged)
        {
            hasStatusChanged = false;
            if (!_require.CheckMessage(message)) return false;
            hasStatusChanged = true;
            return UseMessage(message);
        }

        /// <summary>应用某条消息并返回当前需求是否已经完成</summary>
        /// <param name="message">目标消息</param>
        /// <returns></returns>
        protected abstract bool UseMessage(T message);

#if UNITY_EDITOR
        public MissionRequire<T> Require => _require;
#endif
    }
}
