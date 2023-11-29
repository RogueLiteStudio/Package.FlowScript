using System;

namespace Flow
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FlowNodeCatalogAttribute : Attribute
    {
        public string Name { get; private set; }
        public FlowNodeCatalogAttribute(string name)
        {
            Name = name;
        }
    }
}
