using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Flow
{
    public static class GraphViewEditorUtil
    {
        public static void BuildNodePort(FlowGraphEditor graphEditor, FlowNode node, IFlowNodeView nodeView)
        {
            var view = nodeView as Node;
            if (node.Data is IFlowInputable)
            {
                var inputView = new FlowPortView(true);
                inputView.style.display = DisplayStyle.Flex;
                inputView.GraphEditor = graphEditor;
                inputView.NodeGUID = node.GUID;
                inputView.Index = 0;
                view.inputContainer.Add(inputView);
                nodeView.InputPorts = new FlowPortView[] { inputView };
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
                view.outputContainer.Add(trueView);

                var falseView = new FlowPortView(false);
                falseView.style.display = DisplayStyle.Flex;
                falseView.GraphEditor = graphEditor;
                falseView.NodeGUID = node.GUID;
                falseView.Index = 0;
                falseView.portName = "False";
                falseView.visualClass = "Port_False";
                view.outputContainer.Add(falseView);
                nodeView.OutputPorts = new FlowPortView[] { trueView, falseView };
            }
            else if (node.Data is IFlowOutputable)
            {
                var outputView = new FlowPortView(false);
                outputView.style.display = DisplayStyle.Flex;
                outputView.GraphEditor = graphEditor;
                outputView.NodeGUID = node.GUID;
                outputView.Index = 0;
                view.outputContainer.Add(outputView);
                nodeView.OutputPorts = new FlowPortView[] { outputView };
            }
        }
    }
}