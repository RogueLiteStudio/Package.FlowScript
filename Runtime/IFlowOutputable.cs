namespace Flow
{
    //输出节点
    public interface IFlowOutputable
    {
    }

    //条件节点，有两个输出，0是true，1是false
    public interface IFlowConditionable : IFlowOutputable
    {
    }
}