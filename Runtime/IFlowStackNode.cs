namespace Flow
{
    public interface IFlowStackElement : IFlowNodeData
    {
    }

    public interface IFlowStackNode : IFlowNodeData
    {
        bool Acceptable(IFlowStackElement element);
    }
}