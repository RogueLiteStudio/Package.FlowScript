using System;

namespace Flow
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CustomFlowGraphProcessAttribute : Attribute
    {
        public Type GraphType { get; private set; }

        public CustomFlowGraphProcessAttribute(Type graphType)
        {
            GraphType = graphType;
        }
    }
}
