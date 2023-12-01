using UnityEditor;

namespace Flow
{
    public static class GraphCopyUtil
    {
        public static FlowGraphCopyData CopyToClpboard(FlowGraphEditor editor)
        {
            var copyData = ToCopyData(editor);
            if (copyData != null)
            {
                FlowGraphClipboard.instance.SetCopy(copyData);
            }
            return copyData;
        }

        public static void AddSunGraphToCopyData(FlowSubGraph subGraph, string nodeGUID, FlowGraphCopyData graphCopyData)
        {
            if (subGraph.Nodes.Count == 0)
                return;
            FlowCopySubGraph copyData = new FlowCopySubGraph 
            {
                GUID = subGraph.GUID, 
                BindNode = nodeGUID,
                Name = subGraph.Name, 
                AllowStageNode = subGraph.AllowStageNode,
            };
            graphCopyData.Graphs.Add(copyData);
            foreach (var nodeRef in subGraph.Nodes)
            {
                AddNodeToCopyData(subGraph, graphCopyData, copyData, nodeRef.GUID);
            }
            foreach (var edge in subGraph.Edges)
            {
                AddEdgeToCopyData(subGraph, copyData, edge.GUID);
            }
            foreach (var stack in subGraph.Stacks)
            {
                AddStackToCopyData(subGraph, copyData, stack.GUID);
            }
        }

        public static FlowGraphCopyData ToCopyData(FlowGraphEditor editor)
        {
            var elements = editor.View.CollectSelectedCopyableGraphElements();
            if (elements.Count == 0)
                return null;
            FlowGraphCopyData copyData = new FlowGraphCopyData { GraphScript = MonoScript.FromScriptableObject(editor.Graph.Owner) };

            FlowCopySubGraph graphData = new FlowCopySubGraph();//复制的最上层subgraph只记录节点信息

            foreach (var e in elements)
            {
                switch (e)
                {
                    case FlowNodeView nodeView:
                        AddNodeToCopyData(editor.Graph, copyData, graphData, nodeView.viewDataKey);
                        break;
                    case FlowStackNodeView stackNodeView:
                        AddNodeToCopyData(editor.Graph, copyData, graphData, stackNodeView.viewDataKey);
                        AddStackToCopyData(editor.Graph, graphData, stackNodeView.viewDataKey);
                        break;
                    case FlowEdgeView edgeView:
                        AddEdgeToCopyData(editor.Graph, graphData, edgeView.viewDataKey);
                        break;
                }
            }
            ClearUnUsed(copyData);
            return copyData;
        }

        private static void AddNodeToCopyData(FlowSubGraph subGraph, FlowGraphCopyData graphCopyData, FlowCopySubGraph copyData, string guid)
        {
            var node = subGraph.Owner.FindNode(guid);
            if (node == null)
                return;
            var copyNode = new FlowCopyNode 
            { 
                GUID = node.GUID, 
                Name = node.Name, 
                Comment = node.Comment,
                IsStageNode = node.Data is IFlowStackNode,
                DataJson = TypeSerializerHelper.Serialize(node.Data),
            };
            var viewData = subGraph.NodeViews.Find(it => it.NodeGUID == guid);
            if (viewData != null)
            {
                copyNode.Expanded = viewData.Expanded;
                copyNode.Position = viewData.Position;
            }
            copyData.Nodes.Add(copyNode);

            //将绑定的子图也加入复制数据
            var bind = subGraph.Owner.GraphBinds.Find(it => it.NodeGUID == guid);
            if (bind != null)
            {
                var bindSubGraph = subGraph.Owner.FindSubGraph(bind.GraphGUID);
                if (bindSubGraph != null)
                {
                    AddSunGraphToCopyData(subGraph, guid, graphCopyData);
                }
            }
        }

        private static void AddEdgeToCopyData(FlowSubGraph subGraph, FlowCopySubGraph copyData, string guid)
        {
            var edge = subGraph.Edges.Find(it=>it.GUID == guid);
            if (edge == null)
                return;
            //这里先直接加入，创建完最后清理
            copyData.Edges.Add(new FlowCopyEdge 
            { 
                FromeNode = edge.FromeNode, 
                OutPort = edge.OutPort, 
                ToNode = edge.ToNode, 
                InPort = edge.InPort 
            });
        }

        private static void AddStackToCopyData(FlowSubGraph subGraph, FlowCopySubGraph copyData, string guid)
        {
            var stack = subGraph.Stacks.Find(it=>it.GUID == guid);
            if (stack == null || stack.Nodes.Count == 0)
                return;
            var copyStack = new FlowCopyStack { GUID = stack.GUID };
            copyStack.Nodes.AddRange(stack.Nodes);
            copyData.Stacks.Add(copyStack);
        }
        

        private static void ClearUnUsed(FlowGraphCopyData copyData)
        {
            var subGraph = copyData.Graphs[0];
            for (int i= subGraph.Edges.Count-1; i>-0; --i)
            {
                var edge = subGraph.Edges[i];
                if (!subGraph.Nodes.Exists(it => it.GUID == edge.FromeNode) || !subGraph.Nodes.Exists(it => it.GUID == edge.ToNode))
                {
                    //如果连接的某个节点没有被选择，删除该连接
                    subGraph.Edges.RemoveAt(i);
                }
            }
        }
    }
}
