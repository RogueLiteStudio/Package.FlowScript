using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flow
{
    public class GraphNodeEditorUtil
    {
        public static void OnNodePosition(FlowGraphEditor graphEditor, string nodeGUID, Rect position)
        {
            var node = graphEditor.Graph.FindNode(nodeGUID);
            if (node != null)
            {
                //GraphEditorUtil.RegisterUndo(graphEditor.Graph.Owner, "move node");
                node.Position = position;
            }
        }

        public static void OnNodeExpanded(FlowGraphEditor graphEditor, string nodeGUID, bool expanded)
        {
            var node = graphEditor.Graph.FindNode(nodeGUID);
            if (node != null)
            {
                //GraphEditorUtil.RegisterUndo(graphEditor.Graph.Owner, "node expanded");
                node.Expanded = expanded;
            }
        }

        public static void OnNodeSelected(FlowGraphEditor graphEditor, string nodeGUID, bool selected)
        {
            var index = graphEditor.Graph.Nodes.FindIndex(it => it.GUID == nodeGUID);
            if (index >= 0)
            {
                if (selected)
                {
                    if (!graphEditor.SelectNodes.Contains(nodeGUID))
                        graphEditor.SelectNodes.Add(nodeGUID);
                }
                else
                {
                    graphEditor.SelectNodes.Remove(nodeGUID);
                }
            }
        }

        public static void OnInsterNodeToStack(FlowGraphEditor graphEditor, string stackGUID, string nodeGUID, int index)
        {
            if (!graphEditor.Graph.Nodes.Exists(it => it.GUID == stackGUID))
                return;
            if (!graphEditor.Graph.Nodes.Exists(it => it.GUID == nodeGUID))
                return;

            var stack = graphEditor.Graph.Stacks.Find(it => it.GUID == stackGUID);
            int preIndex = -1;
            if (stack != null)
            {
                preIndex = stack.Nodes.FindIndex(it => it == nodeGUID);
                if (preIndex == index)
                    return;
            }
            GraphEditorUtil.RegisterUndo(graphEditor.Graph.Owner, "move in");
            if (stack == null)
            {
                stack = new FlowStackData { GUID = stackGUID };
                graphEditor.Graph.Stacks.Add(stack);
            }
            if (preIndex >= 0)
                stack.Nodes.RemoveAt(preIndex);
            stack.Nodes.Insert(index, nodeGUID);
        }

        public static void OnRemoveNodesFromStack(FlowGraphEditor graphEditor, string stackGUID, IEnumerable<string> nodes)
        {
            if (nodes.Count() > 0)
            {
                var stack = graphEditor.Graph.Stacks.Find(it=>it.GUID == stackGUID);
                GraphEditorUtil.RegisterUndo(graphEditor.Graph.Owner, "move out");
                foreach (var guid in nodes)
                {
                    stack.Nodes.Remove(guid);
                }
            }
        }
    }
}
