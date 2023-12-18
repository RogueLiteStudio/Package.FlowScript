using System;

namespace Flow
{
    public interface IFlowGraphProcess
    {
        Type EditorWindowType { get; }
        FlowGraph OnCreateAction();

        bool CheckIsValidNodeType(Type type);

        void OnCreateSubGraph(FlowGraph graph, FlowSubGraph subGraph, FlowSubGraph prarent, FlowNode bindNode);

        void OnSave(FlowGraph graph);
    }
}
