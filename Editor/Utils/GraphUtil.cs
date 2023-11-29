namespace Flow
{
    public class GraphUtil : GraphDeleteUtil
    {
        public static void AddNodeToGroup(FlowSubGraph subGraph, FlowNodeGroup group, FlowNode node)
        {
            if (group.Nodes.Contains(node.GUID))
                return;
            foreach (var g in subGraph.Groups)
            {
                int idx = g.Nodes.IndexOf(node.GUID);
                if (idx >= 0)
                {
                    g.Nodes.RemoveAt(idx);
                    break;
                }
            }
            group.Nodes.Add(node.GUID);
        }

        public static void RemoveNodeFromGroup(FlowSubGraph subGraph, FlowNode node)
        {
            foreach (var group in subGraph.Groups)
            {
                int idx = group.Nodes.IndexOf(node.GUID);
                if (idx >= 0)
                {
                    group.Nodes.RemoveAt(idx);
                    return;
                }
            }
        }

        public static void RemoveInvalidEdge(FlowGraph graph)
        {
            foreach (var sub in graph.SubGraphs)
            {
                foreach (var nodeRef in sub.Nodes)
                {
                    int maxInPort = nodeRef.Node.Data is IFlowInputable ? 0 : -1;
                    int maxOutPort = nodeRef.Node.Data is IFlowOutputable ? 0 : -1;
                    if (nodeRef.Node.Data is IFlowConditionable)
                        maxOutPort = 1;
                    sub.Edges.RemoveAll(it => (it.FromeNode == nodeRef.Node.GUID && it.OutPort > maxOutPort) 
                    || (it.ToNode == nodeRef.Node.GUID && it.InPort > maxInPort));
                }
            }
        }
    }
}
