namespace Flow
{
    public interface IFlowNodeView
    {
        FlowPortView[] InputPorts { get; set; }
        FlowPortView[] OutputPorts { get; set; }
        string GUID { get; }
        void RefreshNodeView(FlowNode node);
    }
}