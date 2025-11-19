using UnityEngine;

namespace Gameplay.MissionSystem
{
    public class DefaultMissionProperty : MissionProperty
    {
        [SerializeField] private string title;
        [SerializeField] private string description;

        /// <summary>
        /// true = 会被创建，但玩家看不到日志/追踪
        /// </summary>
        [SerializeField] private bool isImplicit;

        public DefaultMissionProperty()
        {
            isImplicit = false;
            title = "Mission Title";
            description = "Mission Description";
        }

#if UNITY_EDITOR
        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public bool IsImplicit { get => isImplicit; set => isImplicit = value; }
#endif
    }
}
