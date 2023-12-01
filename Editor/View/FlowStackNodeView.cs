
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flow
{
    public class FlowStackNodeView : StackNode, IFlowNodeView
    {
        public FlowGraphEditor GraphEditor;
        public FlowPortView[] InputPorts { get; set; }
        public FlowPortView[] OutputPorts { get; set; }
        public string GUID => viewDataKey;
        private List<FlowNodeView> children = new List<FlowNodeView>();
        readonly string styleSheet = "FlowGraphStyles/StackNodeView";
        private Label titleLabel;
        public FlowStackNodeView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(styleSheet));
            headerContainer.Add(titleLabel = new Label());
        }

        public void RefreshNodeView(FlowNode node)
        {
            titleLabel.text = node.Name;
            base.SetPosition(node.Position);
            expanded = node.Expanded;
        }
        public void BindNode(FlowGraphEditor graphEditor, FlowNode node)
        {
            if (node.Data is IFlowEntry)
            {
                capabilities &= ~(Capabilities.Deletable & Capabilities.Copiable);
            }
            GraphEditor = graphEditor;
            viewDataKey = node.GUID;
            RefreshNodeView(node);
            GraphViewEditorUtil.BuildNodePort(graphEditor, node, this);
            RefreshExpandedState();
            RefreshPorts();
        }

        public void RemoveChildren()
        {
            foreach (var child in children)
            {
                RemoveElement(child);
            }
            children.Clear();
        }

        public void AddChild(FlowNodeView child)
        {
            children.Add(child);
            AddElement(child);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            GraphNodeEditorUtil.OnNodePosition(GraphEditor, viewDataKey, newPos);
        }

        protected override bool AcceptsElement(GraphElement element, ref int proposedIndex, int maxIndex)
        {
            if (!base.AcceptsElement(element, ref proposedIndex, maxIndex))
                return false;
            var stackNode = GraphEditor.Graph.FindNode(viewDataKey);
            if (stackNode != null && stackNode.Data is IFlowStackNode stack)
            {
                if (element is FlowNodeView nodeView)
                {
                    var node = GraphEditor.Graph.FindNode(nodeView.viewDataKey);
                    if (node != null && node.Data is IFlowStackElement stackElement)
                    {
                        if (stack.Acceptable(stackElement))
                        {
                            int index = UnityEngine.Mathf.Clamp(proposedIndex, 0, children.Count - 1);
                            children.RemoveAll(it => it.viewDataKey == nodeView.viewDataKey);
                            children.Insert(index, nodeView);
                            GraphNodeEditorUtil.OnInsterNodeToStack(GraphEditor, viewDataKey, nodeView.viewDataKey, index);

                            return true;
                        }
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
            foreach (var guid in guids)
            {
                children.RemoveAll(it => it.viewDataKey == guid);
            }
            GraphNodeEditorUtil.OnRemoveNodesFromStack(GraphEditor, viewDataKey, guids);
            return true;
        }
    }
}