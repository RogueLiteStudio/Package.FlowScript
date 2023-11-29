namespace Flow
{
    public class GraphDeleteUtil : GraphCreateUtil
    {

        public static void RemoveNode(FlowSubGraph subGraph, string guid)
        {
            var index = subGraph.Nodes.FindIndex(it => it.GUID == guid);
            if (index < 0)
                return;
            subGraph.Nodes.RemoveAt(index);
            subGraph.NodeViews.RemoveAll(it => it.NodeGUID == guid);
            subGraph.Edges.RemoveAll(it => it.FromeNode == guid || it.ToNode == guid);
            foreach (var group in subGraph.Groups)
            {
                int idx = group.Nodes.IndexOf(guid);
                if (idx >= 0)
                {
                    group.Nodes.RemoveAt(idx);
                    break;
                }
            }

            var bind = subGraph.Owner.GraphBinds.Find(it => it.NodeGUID == guid);
            if (bind != null)
            {
                RemoveSubGraph(subGraph.Owner, bind.GraphGUID);
            }

        }

        public static void RemoveSubGraph(FlowGraph graph, string subGraphID)
        {
            int index = graph.SubGraphs.FindIndex(it => it.GUID == subGraphID);
            if (index < 0)
                return;
            var subGraph = graph.SubGraphs[index];
            graph.SubGraphs.RemoveAt(index);

            int bindIndex = graph.GraphBinds.FindIndex(it => it.GraphGUID == subGraphID);
            if (bindIndex >= 0)
            {
                graph.GraphBinds.RemoveAt(bindIndex);
            }
            if (subGraph.Nodes.Count > 0)
            {
                //此处防止循环调用
                var nodes = subGraph.Nodes.ToArray();
                foreach (var node in nodes)
                {
                    RemoveNode(subGraph, node.GUID);
                }
            }
        }

        public static void RemoveGroup(FlowSubGraph subGraph, string groupID)
        {
            int index = subGraph.Groups.FindIndex(it => it.GUID == groupID);
            if (index < 0)
                return;
            subGraph.Groups.RemoveAt(index);
        }

        public static void RemoveEdge(FlowSubGraph subGraph, string edgeGUID)
        {
            int idx = subGraph.Edges.FindIndex(it => it.GUID == edgeGUID);
            if (idx >= 0)
                subGraph.Edges.RemoveAt(idx);
        }
    }
}
