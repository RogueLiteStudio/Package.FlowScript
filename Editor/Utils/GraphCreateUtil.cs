using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Flow
{
    public class GraphCreateUtil
    {

        public static FlowSubGraph CreateSubGraph(FlowGraph graph, bool allowStageNode, string parenGUID, string nodeGUID)
        {
            FlowSubGraph subGraph = new FlowSubGraph
            {
                Owner = graph,
                ParentGUID = parenGUID,
                BindNodeGUID = nodeGUID,
                AllowStageNode = allowStageNode,
                GUID = System.Guid.NewGuid().ToString(),
            };
            graph.SubGraphs.Add(subGraph);
            return subGraph;
        }

        public static FlowNode CreateNode(FlowSubGraph subGraph, IFlowNodeData data, Rect position, bool expanded = true)
        {
            FlowNode node = new FlowNode
            { 
                GUID = System.Guid.NewGuid().ToString(),
                Position = position,
                Expanded = expanded
            };
            var nameAttri = data.GetType().GetCustomAttribute<FlowNodeNameAttribute>();
            if (nameAttri != null)
                node.Name = nameAttri.Name;
            else
                node.Name = data.GetType().Name;

            node.SetData(data);

            subGraph.Nodes.Add(node);
            return node;
        }

        public static FlowNodeGroup CreateGroup(FlowSubGraph subGraph, Rect position)
        {
            FlowNodeGroup group = new FlowNodeGroup
            {
                GUID = System.Guid.NewGuid().ToString(),
                Name = "New Group",
                Position = position,
            };
            subGraph.Groups.Add(group);
            return group;
        }

        public static FlowEdgeData CreateEdge(FlowSubGraph subGraph, string fromNodeGUID, int outPort, string toNodeGUID, int inPort)
        {
            if (inPort > 0)
                return null;
            var from = subGraph.FindNode(fromNodeGUID);
            var to = subGraph.FindNode(toNodeGUID);
            if (from == null || to == null)
                return null;
            int maxOutPort = from.Data is IFlowConditionable ? 1 : 0;
            if (outPort > maxOutPort)
                return null;
            if (from.Data is not IFlowOutputable || to.Data is not IFlowInputable)
                return null;
            var edge = subGraph.FindEdge(from.GUID, outPort, to.GUID, inPort);
            if (edge == null)
            {
                edge = new FlowEdgeData
                {
                    GUID = System.Guid.NewGuid().ToString(),
                    FromeNode = from.GUID,
                    OutPort = outPort,
                    ToNode = to.GUID,
                    InPort = inPort,
                };
                subGraph.Edges.Add(edge);
            }
            return edge;
        }
    }
}