using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    /// <summary>任务原型对象</summary>
    public class MissionPrototype<T>
    {
        public readonly string id;
        public readonly MissionProperty property;
        public readonly MissionRequire<T>[] requires;
        public readonly MissionRequireMode requireMode;
        public readonly bool isSingleRequire;
        private readonly MissionReward[] rewards;

        /// <summary>
        /// 初始化任务原型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requires"></param>
        /// <param name="rewards"></param>
        /// <param name="requireMode"></param>
        /// <param name="property"></param>
        /// <exception cref="Exception"></exception>
        public MissionPrototype(string id, [DisallowNull] MissionRequire<T>[] requires, MissionReward[] rewards = null, MissionRequireMode requireMode = default, MissionProperty property = null)
        {
            /* check if mission id is valid */
            if (string.IsNullOrEmpty(id))
                throw new Exception("mission id cannot be null or empty");
            this.id = id;

            /* check if require array is valid */
            if (requires == null || requires.Length == 0)
                throw new Exception("mission requires cannot be null or empty");

            this.requires = requires;
            this.rewards = rewards;
            this.requireMode = requireMode;
            this.property = property;

            isSingleRequire = requires.Length == 1;
        }

        /// <summary>兑现所有的奖励</summary>
        public void ApplyReward()
        {
            if (rewards is null || rewards.Length == 0) return;
            foreach (var reward in rewards)
                reward.ApplyReward();
        }

#if UNITY_EDITOR
        // 给 Inspector 看总共有哪些需求
        public IReadOnlyList<MissionRequire<T>> EditorRequires => requires;
#endif
    }
}
