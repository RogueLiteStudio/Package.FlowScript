using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flow
{
    public class GraphNodeEditorUtil
    {
        public static void OnNodePosition(FlowGraphEditor graphEditor, string nodeGUID, Rect position)
        {
            var nodeView = graphEditor.Graph.NodeViews.Find(it => it.NodeGUID == nodeGUID);
            if (nodeView != null)
            {
                GraphEditorUtil.RegisterUndo(graphEditor.Graph.Owner, "move node");
                nodeView.Position = position;
            }
        }

        public static void OnNodeExpanded(FlowGraphEditor graphEditor, string nodeGUID, bool expanded)
        {
            var nodeView = graphEditor.Graph.NodeViews.Find(it => it.NodeGUID == nodeGUID);
            if (nodeView != null)
            {
                GraphEditorUtil.RegisterUndo(graphEditor.Graph.Owner, "node expanded");
                nodeView.Expanded = expanded;
            }
        }

        public static void OnNodeSelected(FlowGraphEditor graphEditor, string nodeGUID, bool selected)
        {
            var index = graphEditor.Graph.Nodes.FindIndex(it => it.GUID == nodeGUID);
            if (index >= 0)
            {
                var nodeRef = graphEditor.Graph.Nodes[index];
                if (selected)
                {
                    graphEditor.SelectNodes.Add(nodeRef);
                }
                else
                {
                    graphEditor.SelectNodes.Remove(nodeRef);
                }
            }
        }

        public static void OnInsterNodeToStack(FlowGraphEditor graphEditor, string stackGUID, string nodeGUID, int index)
        {
            if (!graphEditor.Graph.Nodes.Exists(it => it.GUID == stackGUID))
                return;
            if (!graphEditor.Graph.Nodes.Exists(it => it.GUID == nodeGUID))
                return;

            GraphEditorUtil.RegisterUndo(graphEditor.Graph.Owner, "move in");
            var stack = graphEditor.Graph.Stacks.Find(it => it.GUID == stackGUID);
            if (stack == null)
            {
                stack = new FlowStackData { GUID = stackGUID };
                graphEditor.Graph.Stacks.Add(stack);
            }
            index = Mathf.Clamp(index, 0, stack.Nodes.Count - 1);
            stack.Nodes.Remove(nodeGUID);
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
