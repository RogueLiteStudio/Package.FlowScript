using System.Linq;

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
            subGraph.Edges.RemoveAll(it => it.FromeNode == guid || it.ToNode == guid);
            for (int i=subGraph.Stacks.Count-1; i>=0; --i)
            {
                var stack = subGraph.Stacks[i];
                if (stack.GUID == guid)
                {
                    subGraph.Stacks.RemoveAt(i);
                    break;
                }
                int idx = stack.Nodes.IndexOf(guid);
                if (idx >= 0)
                {
                    stack.Nodes.RemoveAt(i);
                    break;
                }
            }
            foreach (var group in subGraph.Groups)
            {
                int idx = group.Nodes.IndexOf(guid);
                if (idx >= 0)
                {
                    group.Nodes.RemoveAt(idx);
                    break;
                }
            }

            foreach (var sub in subGraph.Owner.SubGraphs)
            {
                if (sub.BindNodeGUID == guid)
                {
                    RemoveSubGraph(subGraph.Owner, sub.GUID);
                    break;
                }
            }
        }

        public static void RemoveSubGraph(FlowGraph graph, string subGraphID)
        {
            int index = graph.SubGraphs.FindIndex(it => it.GUID == subGraphID);
            if (index < 0)
                return;
            var subGraph = graph.SubGraphs[index];
            graph.SubGraphs.RemoveAt(index);
            var subs = graph.SubGraphs.Where(it => it.ParentGUID == subGraphID).Select(it => it.GUID).ToArray();
            foreach (var id in subs)
            {
                RemoveSubGraph(graph, id);
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
