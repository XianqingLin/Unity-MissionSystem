#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;

namespace Gameplay.MissionSystem
{
    public static class UtilsGUI
    {
        /// <summary>
        /// 带标题的折叠 Box（仅编辑器用）
        /// </summary>
        public static void DrawFoldoutBox(string title, Action drawInside)
        {
            GUILayout.Label($"<color=#fffde3><size=12><b>{title}</b></size></color>");
            GUILayout.BeginVertical("box");
            drawInside?.Invoke();
            GUILayout.EndVertical();
            EditorUtils.BoldSeparator();
        }

        /// <summary>
        /// 通用：列表 + 添加按钮区域（仅编辑器用）
        /// </summary>
        public static void DrawListSection<T>(
            string title,
            List<T> list,
            Action<T> drawElement,
            Action addCallback) where T : class
        {
            DrawFoldoutBox(title, () =>
            {
                EditorUtils.ReorderableList(list, (idx, picked) =>
                {
                    var elem = list[idx];
                    if (elem != null) drawElement(elem);
                });

                GUI.backgroundColor = Colors.lightBlue;
                if (GUILayout.Button($"Add {title.Replace(" List", "")}"))
                    addCallback();
                GUI.backgroundColor = Color.white;
            });
        }

        /// <summary>
        /// 弹出类型选择器并实例化添加（仅编辑器用）
        /// </summary>
        public static void PickAndAdd<T>(Action<T> add) where T : class
        {
            Action<Type> onPick = t =>
            {
                var item = (T)Activator.CreateInstance(t);
                add(item);
            };
            var menu = EditorUtils.GetTypeSelectionMenu(typeof(T), onPick);
            menu.ShowAsBrowser($"Select {typeof(T).Name}", typeof(T));
        }
    }
}
#endif