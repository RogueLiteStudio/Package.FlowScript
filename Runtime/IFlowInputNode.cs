namespace Flow
{
    public interface IFlowInputNode
    {
        int InputCount { get; }
        string InputName(int index);
    }
}