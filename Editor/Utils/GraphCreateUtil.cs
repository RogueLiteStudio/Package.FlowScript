using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Flow
{
    public class GraphCreateUtil
    {

        public static FlowSubGraph CreateSubGraph(FlowGraph graph, bool allowStageNode)
        {
            FlowSubGraph subGraph = new FlowSubGraph
            {
                Owner = graph,
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
            };
            var nameAttri = data.GetType().GetCustomAttribute<FlowNodeNameAttribute>();
            if (nameAttri != null)
                node.Name = nameAttri.Name;
            else
                node.Name = data.GetType().Name;

            node.SetData(data);

            subGraph.Owner.Nodes.Add(node);

            subGraph.Nodes.Add(FlowNodeRef.CreateNodeRef(subGraph.Owner, node.GUID));
            subGraph.NodeViews.Add(new FlowNodeViewData { NodeGUID = node.GUID, Position = position, Expanded = expanded });
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

        public static void BindSubGraphToNode(FlowSubGraph subGraph, FlowNode node)
        {
            if (node.Data is not IFlowBindSubGraphable)
                return;
            //第一个为主Graph，不能和节点绑定
            if (subGraph.Owner.SubGraphs[0].GUID == subGraph.GUID)
                return;
            //节点不再当前Graph中，也不能绑定
            if (!subGraph.Owner.HasNode(node.GUID))
                return;
            //如果已经存在绑定关系，也不能绑定
            if (subGraph.Owner.GraphBinds.Exists(it => it.NodeGUID == node.GUID || it.GraphGUID == subGraph.GUID))
                return;
            SubGraphBind bind = new SubGraphBind { NodeGUID = node.GUID, GraphGUID = subGraph.GUID };
            subGraph.Owner.GraphBinds.Add(bind);
        }

        public static FlowEdgeData CreateEdge(FlowSubGraph subGraph, string fromNodeGUID, int outPort, string toNodeGUID, int inPort)
        {
            if (inPort > 0)
                return null;
            var from = subGraph.Owner.FindNode(fromNodeGUID);
            var to = subGraph.Owner.FindNode(toNodeGUID);
            int maxOutPort = from.Data is IFlowConditionable ? 1 : 0;
            if (outPort > maxOutPort)
                return null;
            if (from.Data is not IFlowOutputable || to.Data is not IFlowInputable)
                return null;
            if (!subGraph.HasNode(from) || !subGraph.HasNode(to))
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