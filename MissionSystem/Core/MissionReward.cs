namespace Gameplay.MissionSystem
{
    /// <summary>任务奖励</summary>
    public abstract class MissionReward
    {
        /// <summary>兑现玩家的奖励</summary>
        public abstract void ApplyReward();
    }
}
