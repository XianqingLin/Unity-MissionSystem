using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gameplay.MissionSystem.ConnectionBase;


namespace Gameplay.MissionSystem
{
    public class MissionChainHandle
    {
        private readonly MissionChain chain;
        private readonly Dictionary<string, NodeMission> activeNodes = new();
        private readonly Queue<NodeMission> buffer = new();

        public bool IsCompleted => activeNodes.Count == 0 && buffer.Count == 0;
        public MissionChainHandle(MissionChain chain)
        {
            this.chain = chain;

            /* execute prime node */
            if (chain.primeNode != null)
                ExecuteNode(chain.primeNode as NodeBase);
        }

        public void FlushBuffer(System.Action<MissionPrototype<object>> deployer)
        {
            if (buffer.Count == 0) return;
            while (buffer.Count > 0)
            {
                var node = buffer.Dequeue();
                var missionProto = node.BuildPrototype();
                activeNodes.Add(missionProto.id, node);
                deployer(missionProto);
            }
        }

        public void OnMissionStart(string missionId)
        {
            if (!activeNodes.TryGetValue(missionId, out var node)) return;
            ExecuteSuccessors(node, AdvanceMode.OnStart);
        }

        public void OnMissionComplete(string missionId, bool continues)
        {
            if (!activeNodes.Remove(missionId, out var node)) return;
            if (continues) ExecuteSuccessors(node, AdvanceMode.OnComplete);
        }

        /// <summary>execute given node</summary>
        public void ExecuteNode(NodeBase node)
        {
            if (node is null) return;
            switch (node)
            {
                /* execute action node */
                case NodeAction actionNode:
                    actionNode.Execute();
                    break;

                /* execute mission node, add output prototype to buffer queue */
                case NodeMission missionNode:
                    if (activeNodes.ContainsKey(missionNode.MissionId)) return;
                    buffer.Enqueue(missionNode);
                    break;
            }
        }

        /// <summary>
        /// 根据推进时机（OnStart / OnComplete）批量执行下游节点。
        /// 自身节点已完成/已启动，因此只筛选时机与条件。
        /// </summary>
        /// <param name="fromNode">来源节点</param>
        /// <param name="mode">推进时机</param>
        private void ExecuteSuccessors(NodeBase fromNode, AdvanceMode mode)
        {
            foreach (var outConn in fromNode.outConnections
                                    .OfType<ConnectionBase>()
                                    .Where(c => c.Mode == mode && c.IsAvailable))
            {
                ExecuteNode(outConn.targetNode as NodeBase);
            }
        }
    }
}
