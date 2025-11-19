using UnityEditor;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    public class NC_ItemMissionReward : NCMissionReward
    {
        [SerializeField] private ItemMissionReward core = new();

        public override MissionReward Get()
        {
            return core;
        }

#if UNITY_EDITOR
        public override string Summary
        {
            get
            {
                return $"给予<b><size=12><color=#fffde3> \"{core.ItemName}\" </color></size></b> | 数量为<size=12><b><color=#b1d480> {core.Amount} </color></b></size>";
            }
        }

        public override string Description
        {
            get
            {
                return "道具类型奖励";
            }
        }

        protected override void OnInspectorGUI()
        {
            core.ItemName = EditorGUILayout.TextField("道具名称", core.ItemName);
            core.Amount = EditorGUILayout.IntField("数量", core.Amount);
            core.Param = EditorGUILayout.TextField("扩展参数", core.Param);
        }
#endif
    }
}
