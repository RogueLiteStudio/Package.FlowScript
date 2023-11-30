using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flow
{
    public class FlowNodeView : Node, IFlowNodeView
    {
        public FlowGraphEditor GraphEditor;
        public FlowPortView[] InputPorts { get; set; }
        public FlowPortView[] OutputPorts { get; set; }
        public string GUID => viewDataKey;

        private static readonly string baseNodeStyle = "FlowGraphStyles/NodeView";
        public FlowNodeView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(baseNodeStyle));
        }

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

        protected override void ToggleCollapse()
        {
            base.ToggleCollapse();
            GraphNodeEditorUtil.OnNodeExpanded(GraphEditor, viewDataKey, expanded);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            GraphNodeEditorUtil.OnNodePosition(GraphEditor, viewDataKey, newPos);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            GraphNodeEditorUtil.OnNodeSelected(GraphEditor, viewDataKey, true);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            GraphNodeEditorUtil.OnNodeSelected(GraphEditor, viewDataKey, false);
        }
    }
}
