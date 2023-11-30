using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flow
{
    public class FlowNodeView : Node
    {
        public FlowGraphEditor GraphEditor;
        public FlowPortView[] InputPorts { get; private set; }
        public FlowPortView[] OutputPorts { get; private set; }

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

            if (node.Data is IFlowInputable)
            {
                var inputView = new FlowPortView(true);
                inputView.style.display = DisplayStyle.Flex;
                inputView.GraphEditor = graphEditor;
                inputView.NodeGUID = node.GUID;
                inputView.Index = 0;
                inputContainer.Add(inputView);
                InputPorts = new FlowPortView[] { inputView };
            }
            if (node.Data is IFlowConditionable)
            {
                var trueView = new FlowPortView(false);
                trueView.style.display = DisplayStyle.Flex;
                trueView.GraphEditor = graphEditor;
                trueView.NodeGUID = node.GUID;
                trueView.Index = 0;
                trueView.portName = "True";
                trueView.visualClass = "Port_True";
                outputContainer.Add(trueView);

                var falseView = new FlowPortView(false);
                falseView.style.display = DisplayStyle.Flex;
                falseView.GraphEditor = graphEditor;
                falseView.NodeGUID = node.GUID;
                falseView.Index = 0;
                falseView.portName = "False";
                falseView.visualClass = "Port_False";
                outputContainer.Add(falseView);
                OutputPorts = new FlowPortView[] { trueView, falseView };
            }
            else if (node.Data is IFlowOutputable)
            {
                var outputView = new FlowPortView(false);
                outputView.style.display = DisplayStyle.Flex;
                outputView.GraphEditor = graphEditor;
                outputView.NodeGUID = node.GUID;
                outputView.Index = 0;
                outputContainer.Add(outputView);
                OutputPorts = new FlowPortView[] { outputView };
            }
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
