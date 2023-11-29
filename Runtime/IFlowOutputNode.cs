namespace Flow
{
    public interface IFlowOutputNode
    {
        int OutputCount { get; }
        string OutputName(int index);
    }
}