
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Flow
{
    public class FlowStackNodeView : StackNode, IFlowNodeView
    {
        public FlowGraphEditor GraphEditor;
        public FlowPortView[] InputPorts { get; set; }
        public FlowPortView[] OutputPorts { get; set; }
        public string GUID => viewDataKey;

        public void RefreshNodeView(FlowNode node)
        {
            title = node.Name;
            var viewData = GraphEditor.Graph.NodeViews.Find(it => it.NodeGUID == node.GUID);
            base.SetPosition(viewData.Position);
            expanded = viewData.Expanded;
        }
        public void BindNode(FlowGraphEditor graphEditor, FlowNode node)
        {
            GraphEditor = graphEditor;
            viewDataKey = node.GUID;
            RefreshNodeView(node);
            GraphViewEditorUtil.BuildNodePort(graphEditor, node, this);
            RefreshExpandedState();
            RefreshPorts();
        }
        protected override bool AcceptsElement(GraphElement element, ref int proposedIndex, int maxIndex)
        {
            if (!base.AcceptsElement(element, ref proposedIndex, maxIndex))
                return false;
            var stackNode = GraphEditor.Graph.Owner.FindNode(viewDataKey);
            if (stackNode != null && stackNode.Data is IFlowStackNode stack)
            {
                if (element is FlowNodeView nodeView)
                {
                    var node = GraphEditor.Graph.Owner.FindNode(nodeView.viewDataKey);
                    if (node != null && node.Data is IFlowStackElement stackElement)
                    {
                        GraphNodeEditorUtil.OnInsterNodeToStack(GraphEditor, viewDataKey, nodeView.viewDataKey, proposedIndex);
                        return stack.Acceptable(stackElement);
                    }
                }
            }
            return false;
        }
        public override bool DragLeave(DragLeaveEvent evt, IEnumerable<ISelectable> selection, IDropTarget leftTarget, ISelection dragSource)
        {
            if (!base.DragLeave(evt, selection, leftTarget, dragSource))
                return false;
            var guids = selection.Where(it => it is FlowNodeView).Select(it => (it as FlowNodeView).viewDataKey);
            GraphNodeEditorUtil.OnRemoveNodesFromStack(GraphEditor, viewDataKey, guids);
            return true;
        }
    }
}