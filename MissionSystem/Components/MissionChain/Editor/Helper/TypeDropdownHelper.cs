using System;
using System.Linq;

namespace Gameplay.MissionSystem.Editor
{
    public static class TypeDropdownHelper
    {
        /// <summary>
        /// 绘制 Type 选择下拉框，返回用户选中的 Type
        /// </summary>
        public static Type DrawTypeDropdown(string label, Type selectedType, Type baseType = null)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                                 .SelectMany(a => a.GetTypes())
                                 .Where(t => baseType == null || baseType.IsAssignableFrom(t))
                                 .Where(t => !t.IsAbstract && !t.IsInterface)
                                 .OrderBy(t => t.Name)
                                 .ToArray();

            string[] options = types.Select(t => $"{t.Namespace}.{t.Name}").ToArray();
            string current = selectedType == null ? "" : $"{selectedType.Namespace}.{selectedType.Name}";

            // 复用已有 DropdownMenu
            string result = current;
            DropdownMenu.MakeMenu(label, current, options, str => result = str);

            // 反查 Type
            return types.FirstOrDefault(t => $"{t.Namespace}.{t.Name}" == result) ?? selectedType;
        }
    }
}
