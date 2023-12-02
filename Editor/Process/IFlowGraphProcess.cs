using System;

namespace Flow
{
    public interface IFlowGraphProcess
    {
        Type EditorWindowType { get; }
        FlowGraph OnCreateAction();

        void OnCreateSubGraph(FlowGraph graph, FlowSubGraph subGraph, FlowSubGraph prarent, FlowNode bindNode);

        void OnSave(FlowGraph graph);
    }
}
