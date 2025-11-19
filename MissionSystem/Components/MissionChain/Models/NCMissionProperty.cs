using UnityEditor;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    public class NCMissionProperty
    {
        [SerializeField] private DefaultMissionProperty core = new();
        public DefaultMissionProperty Get() => core;

#if UNITY_EDITOR
        public string Title => core.Title;

        public void OnInspectorGUI()
        {
            core.Title = EditorGUILayout.TextField("标题", core.Title);

            EditorGUILayout.LabelField("描述");
            core.Description = EditorGUILayout.TextArea(core.Description, GUILayout.MinHeight(40));

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("是否为隐式任务");
                core.IsImplicit = EditorGUILayout.Toggle(core.IsImplicit);
            }
            GUILayout.EndHorizontal();
        }
#endif
    }
}
