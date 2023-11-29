using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    /*
     * 子图类型，用于存储子图数据
     * 如果需要扩展子图字段，不要继承该类，在子类的Graph中添一个字段，绑定GUID记录相关的属性即可
     */
    [System.Serializable]
    public class FlowSubGraph
    {
        public FlowGraph Owner;
        public string GUID;
        public string Name;
        public bool AllowStageNode;
        public Vector3 Position;
        public float Scale;
        public List<FlowNodeRef> Nodes = new List<FlowNodeRef>();
        public List<FlowNodeViewData> NodeViews = new List<FlowNodeViewData>();
        public List<FlowEdgeData> Edges = new List<FlowEdgeData>();
        public List<FlowNodeGroup> Groups = new List<FlowNodeGroup>();
        public bool HasNode(FlowNode node)
        {
            return Nodes.Exists(it => it.GUID == node.GUID);
        }

        public FlowEdgeData FindEdge(string guid)
        {
            return Edges.Find(it => it.GUID == guid);
        }

        public FlowEdgeData FindEdge(string fromNode, int outPort, string toNode, int inPort)
        {
            return Edges.Find(it => it.FromeNode == fromNode && it.OutPort == outPort && it.ToNode == toNode && it.InPort == inPort);
        }
    }
}