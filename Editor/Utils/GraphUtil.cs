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
                foreach (var node in sub.Nodes)
                {
                    int maxInPort = node.Data is IFlowInputable ? 0 : -1;
                    int maxOutPort = node.Data is IFlowOutputable ? 0 : -1;
                    if (node.Data is IFlowConditionable)
                        maxOutPort = 1;
                    sub.Edges.RemoveAll(it => (it.FromeNode == node.GUID && it.OutPort > maxOutPort) 
                    || (it.ToNode == node.GUID && it.InPort > maxInPort));
                }
            }
        }
    }
}
