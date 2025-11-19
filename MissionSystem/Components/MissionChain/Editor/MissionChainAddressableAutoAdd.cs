#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    internal sealed class MissionChainAddressableAutoAdd : AssetPostprocessor
    {
        // 资源导入成功后调用
        private static void OnPostprocessAllAssets(
            string[] imported, string[] deleted, string[] moved, string[] movedFromAssetPaths)
        {
            foreach (var path in imported)
                TryAddToAddressables(path);
        }

        private static void TryAddToAddressables(string path)
        {
            if (!path.EndsWith(".asset")) return;

            // 精确类型判断
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (type != typeof(MissionChain)) return;

            var guid = AssetDatabase.AssetPathToGUID(path);
            // 延迟到下一帧，确保文件已完全写入
            EditorApplication.delayCall += () => DoAddToAddressables(guid);
        }

        private static void DoAddToAddressables(string guid)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogWarning("[MissionChain] 项目中没有 Addressables Settings，跳过自动添加。");
                return;
            }

            if (settings.FindAssetEntry(guid) != null) return; // 已存在

            var group = settings.DefaultGroup;
            var entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.address = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid));

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
        }
    }
}

#endif