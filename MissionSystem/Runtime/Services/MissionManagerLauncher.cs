using Cysharp.Threading.Tasks;
using Framework.Event;
using System;
using UnityEngine;

namespace Gameplay.MissionSystem
{
    public class MissionManagerLauncher : MonoBehaviour
    {
        public static MissionManager<object> Instance { get; private set; }

        public static MissionChainManager MissionChainManager;

        void Awake()
        {
            Instance = new MissionManager<object>();
        }

        private void Start()
        {
            MissionChainManager = new MissionChainManager(Instance, ResourceManagerLauncher.Instance);
            Instance.AddComponent(MissionChainManager);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("Start Chain");
                MissionChainManager.StartChainAsync("MissionChain_Main").Forget();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("MSG: CollectItem");
                Instance.SendMessage(new GameMessage(GameEventType.CollectItem, "Toolbox"));
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("MSG: CompleteDialogue");
                Instance.SendMessage(new GameMessage(GameEventType.CompleteDialogue, "Pelican"));
            }
        }

#if UNITY_EDITOR
        public static MissionManager<object> EditorInstance { get => Instance; set => Instance = value; }
#endif
    }
}
