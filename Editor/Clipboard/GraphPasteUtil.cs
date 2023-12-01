using System.Collections.Generic;

namespace Flow
{
    public static class GraphPasteUtil
    {
        //Paste时，GUID映射表<旧GUID, 新GUID>
        private static Dictionary<string, string> sGUIDMap = new Dictionary<string, string>();
        private static Dictionary<string, string> sNodeOwners = new Dictionary<string, string>();
        public static bool CheckPaste(FlowGraphEditor editor)
        {
            var copyData = FlowGraphClipboard.instance.GetCopyData(editor.Graph.Owner);
            if (copyData == null)
                return false;
            return CheckPaste(editor, copyData);
        }

        private static bool CheckPaste(FlowGraphEditor editor, FlowGraphCopyData copyData)
        {
            if (copyData.Graphs.Count == 0 || copyData.Graphs[0].Nodes.Count == 0)
                return false;
            if (copyData.GraphScript.GetType() != editor.Graph.Owner.GetType())
                return false;
            if (!editor.Graph.AllowStageNode && copyData.Graphs[0].Nodes.Exists(it=>it.IsStageNode))
                return false;
            return true;
        }

        public static FlowGraphCopyData GetCopyData(FlowGraph graph)
        {
            return FlowGraphClipboard.instance.GetCopyData(graph);
        }

        public static void Paste(FlowGraphEditor editor, FlowGraphCopyData copyData, string opName)
        {
            sGUIDMap.Clear();
            sNodeOwners.Clear();
            if (!CheckPaste(editor, copyData))
                return;
            GraphEditorUtil.RegisterUndo(editor.Graph.Owner, opName);
            var mainGraphData = copyData.Graphs[0];
            foreach (var data in mainGraphData.Nodes)
            {
                PasteNode(editor.Graph, data);
            }
            foreach (var data in mainGraphData.Stacks)
            {
                PasteStack(editor.Graph, data);
            }
            foreach (var data in mainGraphData.Edges)
            {
                PasteEdge(editor.Graph, data);
            }
            for (int i=1; i<copyData.Graphs.Count; ++i)
            {
                PasteSubGraph(editor.Graph.Owner, copyData.Graphs[i]);

            }
            editor.RefreshView();
        }

        private static FlowNode PasteNode(FlowSubGraph subGraph, FlowCopyNode copyData)
        {
            IFlowNodeData data = TypeSerializerHelper.Deserialize(copyData.DataJson) as IFlowNodeData;
            //重构可能会导致类型丢失
            if (data == null)
                return null;
            //如果已经创建过，直接返回
            if (sGUIDMap.TryGetValue(copyData.GUID, out var newGUID))
            {
                return subGraph.FindNode(newGUID);
            }
            var node = GraphCreateUtil.CreateNode(subGraph, data, copyData.Position, copyData.Expanded);
            sGUIDMap.Add(copyData.GUID, node.GUID);
            sNodeOwners.Add(node.GUID, subGraph.GUID);
            return node;
        }

        private static FlowStackData PasteStack(FlowSubGraph subGraph, FlowCopyStack copyData)
        {
            //如果节点没有创建成功，则不创建Stack
            if (!sGUIDMap.TryGetValue(copyData.GUID, out var newGUID))
                return null;

            FlowStackData stackData = new FlowStackData { GUID = newGUID};
            foreach (var id in copyData.Nodes)
            {
                if (sGUIDMap.TryGetValue(id, out var newId))
                {
                    stackData.Nodes.Add(newId);
                }
            }
            return stackData;
        }

        private static FlowEdgeData PasteEdge(FlowSubGraph subGraph, FlowCopyEdge copyData)
        {
            if (!sGUIDMap.TryGetValue(copyData.FromeNode, out string newFrom))
                return null;
            if (!sGUIDMap.TryGetValue(copyData.ToNode, out string newTo))
                return null;
            FlowEdgeData edgeData = GraphCreateUtil.CreateEdge(subGraph, newFrom, copyData.OutPort, newTo, copyData.InPort);
            return edgeData;
        }

        private static FlowSubGraph PasteSubGraph(FlowGraph graph, FlowCopySubGraph copyData)
        {
            //如果已经创建过，直接返回
            if (sGUIDMap.TryGetValue(copyData.GUID, out var newGUID))
            {
                return graph.FindSubGraph(newGUID);
            }

            if (!sGUIDMap.TryGetValue(copyData.BindNode, out var nodeGUID))
                return null;
            sNodeOwners.TryGetValue(nodeGUID, out var nodeOwnerID);
            var subGraph = GraphCreateUtil.CreateSubGraph(graph, copyData.AllowStageNode, nodeOwnerID, nodeGUID);
            sGUIDMap.Add(copyData.GUID, subGraph.GUID);
            foreach (var data in copyData.Nodes)
            {
                PasteNode(subGraph, data);
            }
            foreach (var data in copyData.Stacks)
            {
                PasteStack(subGraph, data);
            }
            foreach (var data in copyData.Edges)
            {
                PasteEdge(subGraph, data);
            }
            return subGraph;
        }
    }
}
