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
        public string ParentGUID;
        public string BindNodeGUID;
        public string Name;
        public bool AllowStageNode;
        public Vector3 Position;
        public Vector3 Scale;
        public List<FlowNode> Nodes = new List<FlowNode>();
        public List<FlowEdgeData> Edges = new List<FlowEdgeData>();
        public List<FlowNodeGroup> Groups = new List<FlowNodeGroup>();
        public List<FlowStackData> Stacks = new List<FlowStackData>();//延迟创建，只有放入节点时才创建对应的Data
        public bool HasNode(string guid)
        {
            return Nodes.Exists(it => it.GUID == guid);
        }
        public FlowNode FindNode(string guid)
        {
            return Nodes.Find(it => it.GUID == guid);
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