using ParadoxNotion.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    [ParadoxNotion.Design.Icon("Eye"), Color("FFFDE3"), Name("Mission")]
    [Description("setup a new mission")]
    public class NodeMission : NodeBase
    {
        public override bool allowAsPrime => true;

        [SerializeField]
        private readonly List<NCMissionRequire> _requires = new();

        [SerializeField] private MissionRequireMode _mode;

        [SerializeField] private NCMissionProperty _property = new();


        [SerializeField]
        private readonly List<NCMissionReward> _rewards = new();

        public string MissionId => $"{graph.name}.{base.UID}";
        public MissionPrototype<object> BuildPrototype() =>
            new(
                MissionId,
                _requires.Select(t => t.Get()).ToArray(),
                _rewards.Select(t => t.Get()).ToArray(),
                _mode,
                _property.Get()
                );

        /// <summary>add new require to current list</summary>
        public void AddRequire(NCMissionRequire require)
        {
            if (require is null || _requires.Contains(require)) return;
            UndoUtility.RecordObject(graph, "Require Added");
            require._node = this;
            _requires.Add(require);
        }

        /// <summary>remove an existing require from current list</summary>
        public void RemoveRequire(NCMissionRequire require)
        {
            if (require is null || !_requires.Contains(require)) return;
            UndoUtility.RecordObject(graph, "Require Removed");
            _requires.Remove(require);
        }

        public void AddReward(NCMissionReward reward)
        {
            if (reward is null || _rewards.Contains(reward)) return;
            UndoUtility.RecordObject(graph, "Reward Added");
            reward._node = this;
            _rewards.Add(reward);
        }

        public void RemoveReward(NCMissionReward reward)
        {
            if (reward is null || !_rewards.Contains(reward)) return;
            UndoUtility.RecordObject(graph, "Reward Removed");
            _rewards.Remove(reward);
        }

#if UNITY_EDITOR
        public override string name => _property.Title;

        protected override void OnNodeInspectorGUI()
        {
            base.OnNodeInspectorGUI();
            GUILayout.Space(4);

            /* ---------- Mission Property ---------- */
            UtilsGUI.DrawFoldoutBox("Mission Property", _property.OnInspectorGUI);

            /* ---------- Requires ---------- */
            UtilsGUI.DrawListSection(
                "Requires List",
                _requires,
                (req) => req.DrawInspector(),
                () => UtilsGUI.PickAndAdd<NCMissionRequire>(AddRequire)
            );

            if (_requires.Count > 1)
                _mode = (MissionRequireMode)EditorGUILayout.EnumPopup("Require Mode", _mode);

            /* ---------- Rewards ---------- */
            UtilsGUI.DrawListSection(
                "Rewards List",
                _rewards,
                (rw) => rw.DrawInspector(),
                () => UtilsGUI.PickAndAdd<NCMissionReward>(AddReward)
            );
        }

        protected override void OnNodeGUI()
        {
            //限制最小宽高
            GUILayout.Space(1);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(220f);
            EditorGUILayout.EndHorizontal();

            /* ---------- 任务需求 ---------- */
            GUILayout.Label($"<color=#fffde3><size=14><b>任务需求</b></size></color>", Styles.leftLabel);
            GUILayout.BeginVertical(Styles.roundedBox);
            int showMax = 3;
            for (int i = 0; i < Math.Min(showMax, _requires.Count); ++i)
                GUILayout.Label(_requires[i].Summary, Styles.centerLabel);

            if (_requires.Count > showMax)
                GUILayout.Label("<i><color=#969596>....</color></i>", Styles.centerLabel);

            if (_requires.Count == 0)
                GUILayout.Label("<i><color=#969596>No Require..</color></i>", Styles.centerLabel);
            GUILayout.EndVertical();

            /* ---------- 任务奖励 ---------- */
            if (_rewards.Count == 0) return;

            GUILayout.Label($"<color=#fffde3><size=14><b>任务奖励</b></size></color>", Styles.leftLabel);
            GUILayout.BeginVertical(Styles.roundedBox);

            for (int i = 0; i < Math.Min(showMax, _rewards.Count); ++i)
                GUILayout.Label(_rewards[i].Summary, Styles.centerLabel);

            if (_rewards.Count > showMax)
                GUILayout.Label("<i><color=#969596>....</color></i>", Styles.centerLabel);
            GUILayout.EndVertical();
        }
#endif
    }
}
