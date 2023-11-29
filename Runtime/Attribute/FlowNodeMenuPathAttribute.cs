using System;

namespace Flow
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FlowNodeMenuPathAttribute : Attribute
    {
        public string Path { get; private set; }
        public FlowNodeMenuPathAttribute(string path)
        {
            Path = path;
        }
    }
}
