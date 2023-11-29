using UnityEditor;
using UnityEngine;

namespace Flow
{
    public class GraphCreateUtil
    {
        public static void RegisterUndo(FlowGraph graph, string name )
        {
            Undo.RegisterCompleteObjectUndo(graph, name);
            EditorUtility.SetDirty(graph);
        }

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

        public static FlowNode CreateNode(FlowSubGraph subGraph, IFlowNodeData data, Rect position)
        {
            FlowNode node = new FlowNode
            { 
                GUID = System.Guid.NewGuid().ToString(),
            };
            node.SetData(data);

            subGraph.Owner.Nodes.Add(node);

            subGraph.Nodes.Add(FlowNodeRef.CreateNodeRef(subGraph.Owner, node.GUID));
            subGraph.NodeViews.Add(new FlowNodeViewData { NodeGUID = node.GUID, Position = position });
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

        public static FlowEdgeData CreateEdge(FlowSubGraph subGraph, FlowNode from, int outPort, FlowNode to, int inPort)
        {
            if (!subGraph.HasNode(from) || subGraph.HasNode(to))
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