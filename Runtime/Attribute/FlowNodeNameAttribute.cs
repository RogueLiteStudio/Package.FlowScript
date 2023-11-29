using System;
namespace Flow
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FlowNodeNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public FlowNodeNameAttribute(string name)
        {
            Name = name;
        }
    }
}