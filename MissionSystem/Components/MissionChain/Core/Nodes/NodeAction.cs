using ParadoxNotion;
using ParadoxNotion.Design;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    [ParadoxNotion.Design.Icon("Action"), Color("FEE3E0"), Name("Action")]
    [Description("Perform an action in the mission chain.")]
    public class NodeAction : NodeBase
    {
        public override bool allowAsPrime => false;

        /* out connections is forbidden */
        public override int maxOutConnections => 0;
        public override Alignment2x2 commentsAlignment { get { return Alignment2x2.Bottom; } }

        [SerializeField] private ActionBase action;

        /// <summary>execute this node</summary>
        public void Execute() =>
            action?.Execute();

#if UNITY_EDITOR
        public override string name => action is ActionGroup ? "行为组" : "行为";

        /// <summary>delete given action</summary>
        /// <param name="other"></param>
        public void DeleteAction(ActionBase other)
        {
            /* remove action */
            if (other == action)
            {
                UndoUtility.RecordObject(graph, "Action Removed");
                action = null;
                return;
            }

            /* remove action from group */
            if (action is ActionGroup group)
            {
                UndoUtility.RecordObject(graph, "Action Removed");
                group.RemoveAction(other);
                if (group.Count == 1)
                    action = group.First;
            }

            /* do nothing */
        }

        /// <summary>add a new action</summary>
        public void AddAction(ActionBase newAction)
        {
            if (action == null)
            {
                UndoUtility.RecordObject(graph, "Action Assigned");
                action = newAction;
            }
            else
            {
                UndoUtility.RecordObject(graph, "Action Grouped");
                if (action is ActionGroup group)
                    group.AddAction(newAction);
                else
                {
                    var newGroup = new ActionGroup
                    {
                        _node = this,
                        _unfolded = true
                    };
                    newGroup.AddAction(action);
                    newGroup.AddAction(newAction);
                    action = newGroup;
                }
            }
        }

        public void UnGrouped()
        {
            if (action is ActionGroup group)
            {
                UndoUtility.RecordObject(graph, "Action UnGrouped");
                action = group.allActions.First();
                action._node = this;
            }
        }


        protected override void OnNodeInspectorGUI()
        {
            GUI.backgroundColor = Colors.lightBlue;
            var baseType = typeof(ActionBase);
            var label = "Assign " + baseType.Name.SplitCamelCase();
            if (GUILayout.Button(label))
            {
                Action<Type> TaskTypeSelected = (t) =>
                {
                    var newAction = (ActionBase)Activator.CreateInstance(t);
                    newAction._node = this;
                    AddAction(newAction);
                };

                var menu = EditorUtils.GetTypeSelectionMenu(baseType, TaskTypeSelected);
                if (CopyBuffer.TryGetCache<ActionBase>(out var copiedAction))
                {
                    menu.AddSeparator("/");
                    menu.AddItem(new GUIContent($"Paste {copiedAction.Summary}"), false, () =>
                    {
                        AddAction(Utils.CopyObject(copiedAction));
                    });
                }

                menu.ShowAsBrowser(label, baseType);
            }

            GUI.backgroundColor = Color.white;

            action?.DrawInspector();
        }

        protected override void OnNodeGUI()
        {
            GUILayout.BeginVertical(Styles.roundedBox);
            GUILayout.Label(action?.Summary ?? "<i><color=#969596>No Action Assigned..</color></i>", Styles.centerLabel);
            GUILayout.EndVertical();
        }
#endif
    }
}
