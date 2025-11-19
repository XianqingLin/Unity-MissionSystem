using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    [Name("Set Dialogue"), Category("Utils"), Description("Set target's dialogue key.")]
    public class SetDialogue : ActionBase
    {
        [SerializeField] private string target;
        [SerializeField] private string dialogueKey;

        public override void Execute()
        {
            Debug.Log(Summary);
        }

#if UNITY_EDITOR
        public override string Summary
        {
            get
            {
                return $"将 <size=12><b><color=#b1d480>{target}</color></b></size> 的对话键设置为" +
                       $" <size=12><b><color=#2196f3>{dialogueKey}</color></b></size>";
            }
        }

        protected override void OnInspectorGUI()
        {
            target = EditorGUILayout.TextField("目标", target);
            dialogueKey = EditorGUILayout.TextField("对话键", dialogueKey);
        }
#endif
    }
}