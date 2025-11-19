using Framework.Event;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    public class NC_CommonMissionRequire : NCMissionRequire
    {
        [SerializeField] private CommonMissionRequire core = new();
        public override MissionRequireTemplate Get() => core;

#if UNITY_EDITOR
        public override string Summary
        {
            get
            {
                return $"监听<b><size=12><color=#fffde3> \"{core.Type}\" </color></size></b>" +
                       $"事件<size=12><b><color=#b1d480> {core.Count} </color></b></size>次" +
                       $" <size=12><b><color=#2196f3>({core.Args}) </color></b></size>";
            }
        }

        public override string Description
        {
            get
            {
                return "GameEvent 类型需求";
            }
        }

        protected override void OnInspectorGUI()
        {
            core.Type  = (GameEventType)UnityEditor.EditorGUILayout.EnumPopup("事件类型", core.Type);
            core.Args  = UnityEditor.EditorGUILayout.TextField("参数", core.Args);
            core.Count = UnityEditor.EditorGUILayout.IntField("数量", core.Count);
        }
    }
#endif
}
