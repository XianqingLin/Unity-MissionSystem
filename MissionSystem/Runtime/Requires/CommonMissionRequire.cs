using Framework.Event;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    [System.Serializable]
    public class CommonMissionRequire : MissionRequireTemplate
    {
        [SerializeField] private GameEventType type;
        [SerializeField] private string args;
        [SerializeField] private int count;

        public CommonMissionRequire()
        {
            type = GameEventType.Default;
            args = null;
            count = 0;
        }

        public CommonMissionRequire(GameEventType type, string args, int count)
        {
            this.type = type;
            this.args = args;
            this.count = count;
        }

        public override bool CheckMessage(object message)
        {
            if (message is not GameMessage gameMessage) return false;
            return gameMessage.type == type && gameMessage.args?.ToString() == args;
        }

        [System.Serializable]
        public class Handle : MissionRequireTemplateHandle
        {
            private readonly CommonMissionRequire require;
            private int count;

            public Handle(CommonMissionRequire gmRequire) : base(gmRequire)
            {
                require = gmRequire;
            }

            protected override bool UseMessage(object message)
            {
                return ++count >= require.count;
            }

#if UNITY_EDITOR
            public int EditorCurrent => count;
#endif
        }

#if UNITY_EDITOR
        public GameEventType Type { get => type; set => type = value; }
        public string Args { get => args; set => args = value; }
        public int Count { get => count; set => count = value; }


        public int EditorTarget => count;
        public GameEventType EditorEventType => type;
        public string EditorArgs => args;
#endif
    }
}
