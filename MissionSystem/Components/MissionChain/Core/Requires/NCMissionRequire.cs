using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    public abstract class NCMissionRequire : MissionChainEditorObject
    {
        public abstract MissionRequireTemplate Get();

#if UNITY_EDITOR
        public NodeMission _node;
        protected override GenericMenu GetContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy"), false, () => CopyBuffer.SetCache(this));
            menu.AddItem(new GUIContent("Reset"), false, Reset);
            if (CopyBuffer.TryGetCache<NCMissionRequire>(out var cache) && cache != this && cache.GetType() == GetType())
                menu.AddItem(new GUIContent("Paste"), false, () => Utils.CopyObjectFrom(this, cache));
            else
                menu.AddDisabledItem(new GUIContent("Paste"));

            menu.AddSeparator("/");
            menu.AddItem(new GUIContent("Delete"), false, () => _node.RemoveRequire(this));
            menu = OnCreateContextMenu(menu);
            return menu;
        }

        /// <summary>overwrite this function if you try to 
        /// manually reset require template</summary>
        public override void Reset()
        {
            UndoUtility.RecordObject(_node.graph, "Require Reset");
            Utils.ResetObject(this);
        }

        /// <summary>
        /// overwrite this function if you try to 
        /// add more options into context menu
        /// </summary>
        protected virtual GenericMenu OnCreateContextMenu(GenericMenu menu) => menu;
#endif
    }
}
