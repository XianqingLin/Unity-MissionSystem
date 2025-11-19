using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    public abstract class NCMissionReward : MissionChainEditorObject
    {
        public abstract MissionReward Get();
#if UNITY_EDITOR
        public NodeMission _node;
        protected override GenericMenu GetContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy"), false, () => CopyBuffer.SetCache(this));
            menu.AddItem(new GUIContent("Reset"), false, Reset);
            if (CopyBuffer.TryGetCache<NCMissionReward>(out var cache) && cache != this && cache.GetType() == GetType())
                menu.AddItem(new GUIContent("Paste"), false, () => Utils.CopyObjectFrom(this, cache));
            else
                menu.AddDisabledItem(new GUIContent("Paste"));

            menu.AddSeparator("/");
            menu.AddItem(new GUIContent("Delete"), false, () => _node.RemoveReward(this));
            menu = OnCreateContextMenu(menu);
            return menu;
        }

        public override void Reset()
        {
            UndoUtility.RecordObject(_node.graph, "Reward Reset");
            Utils.ResetObject(this);
        }

        protected virtual GenericMenu OnCreateContextMenu(GenericMenu menu) => menu;
#endif
    }
}