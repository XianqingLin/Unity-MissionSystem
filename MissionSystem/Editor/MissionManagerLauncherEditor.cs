#if UNITY_EDITOR
using Gameplay.MissionSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionManagerLauncher))]
public class MissionManagerLauncherEditor : Editor, IMissionSystemComponent<object>
{
    /*====================  运行期常驻的静态数据  ====================*/
    private static readonly Dictionary<string, List<RequirementSnap>> _snapshots = new();
    private static readonly HashSet<string> _finishedIDs = new();
    /*================================================================*/
    private void OnEnable()
    {
        MissionManagerLauncher.Instance?.AddComponent(this);
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        MissionManagerLauncher.Instance?.RemoveComponent(this);
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            MissionManagerLauncher.EditorInstance = null;
            _snapshots.Clear();
            _finishedIDs.Clear();
        }
    }

    /* --------------------  接口实现  -------------------- */
    public void OnMissionStarted(Mission<object> mission) => SessionState.SetBool(mission.Id, true);

    public void OnMissionStatusChanged(Mission<object> mission, bool isFinished)
    {
        TakeSnapshot(mission);
        if (isFinished) _finishedIDs.Add(mission.Id);
    }

    public void OnMissionRemoved(Mission<object> mission, bool isFinished) { }

    /* --------------------  拍照  -------------------- */
    private void TakeSnapshot(Mission<object> mission)
    {
        var title = (mission.Property as DefaultMissionProperty)?.Title ?? mission.Id;

        var snaps = mission.Proto.EditorRequires
            .Select(r => r as CommonMissionRequire)
            .Where(cmr => cmr != null)
            .Select(cmr =>
            {
                var handle = mission.EditorUnfinishedHandles
                                    .FirstOrDefault(h => ReferenceEquals(h.Require, cmr));
                int current = handle is CommonMissionRequire.Handle ch ? ch.EditorCurrent : cmr.EditorTarget;
                return new RequirementSnap(cmr, current, title);
            })
            .ToList();

        _snapshots[mission.Id] = snaps;
    }

    /* --------------------  Inspector  -------------------- */
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("任务进度", EditorStyles.boldLabel);

        var manager = MissionManagerLauncher.Instance;
        if (manager == null)
        {
            EditorGUILayout.HelpBox("任务系统尚未初始化，请进入 Play 模式", MessageType.Info);
            return;
        }

        var missions = manager.GetMissions();
        if (missions.Length == 0)
        {
            EditorGUILayout.HelpBox("当前无进行中的任务", MessageType.Info);
        }
        else
        {
            /* 进行中 */
            foreach (var m in missions)
            {
                if (!_snapshots.ContainsKey(m.Id)) TakeSnapshot(m);
                DrawMission(m.Id, _snapshots[m.Id]);
            }
        }

        /* 已完成 */
        var done = _snapshots.Where(kv => _finishedIDs.Contains(kv.Key)).ToList();
        if (done.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("已完成任务", EditorStyles.boldLabel);
            foreach (var kv in done) DrawMission(kv.Key, kv.Value);
        }

        /* 清除按钮 */
        if (done.Count > 0 && GUILayout.Button("清除已完成缓存"))
        {
            foreach (var id in done.Select(kv => kv.Key).ToArray())
            {
                _snapshots.Remove(id);
                _finishedIDs.Remove(id);
            }
        }

        Repaint();
    }

    /* --------------------  UI 绘制  -------------------- */
    private void DrawMission(string id, List<RequirementSnap> snaps)
    {
        if (snaps.Count == 0) return;
        string title = snaps[0].Title;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        bool fold = EditorGUILayout.Foldout(SessionState.GetBool(id, false), title, true);
        SessionState.SetBool(id, fold);

        if (fold)
        {
            EditorGUI.indentLevel++;
            foreach (var s in snaps) DrawRequirementLine(s);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
    }

    private static void DrawRequirementLine(RequirementSnap snap)
    {
        GUI.color = snap.Finished ? Color.green : GUI.color;
        EditorGUILayout.LabelField($"{snap.Desc}  {snap.Current}/{snap.Target}");
        GUI.color = Color.white;
    }

    /* --------------------  数据结构  -------------------- */
    private readonly struct RequirementSnap
    {
        public readonly string Title;
        public readonly string Desc;
        public readonly int Current;
        public readonly int Target;
        public readonly bool Finished;

        public RequirementSnap(CommonMissionRequire cmr, int current, string title)
        {
            Title = title;
            Desc = $"{cmr.EditorEventType}（{cmr.EditorArgs}）";
            Current = current;
            Target = cmr.EditorTarget;
            Finished = current >= cmr.EditorTarget;
        }
    }
}
#endif