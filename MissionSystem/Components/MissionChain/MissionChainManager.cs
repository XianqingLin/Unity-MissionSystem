using Cysharp.Threading.Tasks;
using Framework.ResourceManager;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    public class MissionChainManager : IMissionSystemComponent<object>
    {
        private readonly MissionManager<object> missionManager;
        private readonly Dictionary<string, MissionChainHandle> handles = new();

        private readonly IAssetManager _asset;

        public MissionChainManager(MissionManager<object> missionManager, IAssetManager assetManager)
        {
            this.missionManager = missionManager;
            _asset = assetManager;
        }
        public void OnMissionStarted(Mission<object> mission)
        {
            var missionChainId = mission.Id.Split('.')[0];
            if (!handles.TryGetValue(missionChainId, out var handle)) return;

            handle.OnMissionStart(mission.Id);
            handle.FlushBuffer(t => missionManager.StartMission(t));
        }

        public void OnMissionRemoved(Mission<object> mission, bool isFinished)
        {
            // Get the mission chain handle
            var missionChainId = mission.Id.Split('.')[0];
            if (!handles.TryGetValue(missionChainId, out var handle)) return;

            // Notify the handle that the mission is completed
            handle.OnMissionComplete(mission.Id, isFinished);
            handle.FlushBuffer(t => missionManager.StartMission(t));

            // Remove the handle if the mission is finished
            if (handle.IsCompleted)
            {
                handles.Remove(missionChainId);
                _asset.Release(missionChainId);
            }
        }

        public void OnMissionStatusChanged(Mission<object> mission, bool isFinished) { }

        public async UniTask StartChainAsync(string key)
        {
            if (handles.ContainsKey(key)) return;

            var chain = await _asset.LoadAsync<MissionChain>(key);
            if (chain == null) return;

            var handle = new MissionChainHandle(chain);
            if (handle.IsCompleted)
            {
                _asset.Release(key);
                return;
            }

            handles.Add(key, handle);
            handle.FlushBuffer(t => missionManager.StartMission(t));
        }
    }
}
