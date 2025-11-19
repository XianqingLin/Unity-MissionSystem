using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System;
using UnityEditor;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    public class ConnectionBase : Connection
    {
        [SerializeField] private bool hasCondition;
        [SerializeField] private ConditionBase _condition;

        public enum AdvanceMode { OnComplete, OnStart }   // 0 = 默认完成后

        [SerializeField] private AdvanceMode mode = AdvanceMode.OnComplete;

        public AdvanceMode Mode => mode;
        public bool IsAvailable
        {
            get
            {
                if (!isActive) return false;
                if (!hasCondition || _condition == null) return true;
                return _condition.IsConditionMet;
            }
        }

#if UNITY_EDITOR
        protected override string GetConnectionInfo()
        {
            return $"{mode}";
        }

        protected override void OnConnectionInspectorGUI()
        {
            base.OnConnectionInspectorGUI();
            mode = (AdvanceMode)EditorGUILayout.EnumPopup("Mode", mode);
            hasCondition = EditorGUILayout.Toggle("Has Condition", hasCondition);
            if (!hasCondition) return;

            // Draw the condition field
            if (_condition == null)
            {
                if (GUILayout.Button("Add Condition"))
                {
                    Action<Type> OnConditionSelected = (type) =>
                    {
                        UndoUtility.RecordObject(graph, "Condition Added");
                        _condition = (ConditionBase)Activator.CreateInstance(type);
                    };

                    var menu = EditorUtils.GetTypeSelectionMenu(typeof(ConditionBase), OnConditionSelected);
                    menu.ShowAsBrowser("Select Condition");
                }
            }
            else
            {
                _condition.DrawInspector();
                if (GUILayout.Button("Remove Condition"))
                {
                    _condition = null;
                }
            }
        }
#endif
    }
}
