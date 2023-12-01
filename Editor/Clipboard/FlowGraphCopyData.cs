using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Flow
{
    [Serializable]
    public class FlowCopyNode
    {
        public string GUID;
        public string Name;
        public string Comment;
        public bool IsStageNode;
        public SerializationData DataJson;
        //ViewData
        public Rect Position;
        public bool Expanded;
    }
    [Serializable]
    public class FlowCopyEdge
    {
        public string FromeNode;
        public int OutPort;
        public string ToNode;
        public int InPort;
    }
    [Serializable]
    public class FlowCopyStack
    {
        public string GUID;
        public List<string> Nodes = new List<string>();
    }

    [Serializable]
    public class FlowCopySubGraph
    {
        public string GUID;
        public string BindNode;
        public string Name;
        public bool AllowStageNode;
        //节点信息
        public List<FlowCopyNode> Nodes = new List<FlowCopyNode>();
        public List<FlowCopyEdge> Edges = new List<FlowCopyEdge>();
        public List<FlowCopyStack> Stacks = new List<FlowCopyStack>();
        //暂时不支持
        //public List<FlowNodeGroup> Groups = new List<FlowNodeGroup>();
    }

    [Serializable]
    public class FlowGraphCopyData
    {
        public MonoScript GraphScript;
        public List<FlowCopySubGraph> Graphs = new List<FlowCopySubGraph>();
    }
}
