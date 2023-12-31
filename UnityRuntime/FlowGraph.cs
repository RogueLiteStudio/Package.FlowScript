using System.Collections.Generic;
using UnityEngine;
namespace Flow
{
    public class FlowGraph : ScriptableObject
    {
        [HideInInspector]
        public List<FlowSubGraph> SubGraphs = new List<FlowSubGraph>();

        public FlowSubGraph FindSubGraph(string guid)
        {
            return SubGraphs.Find(it => it.GUID == guid);
        }

        public virtual bool CheckIsValidNodeType(System.Type type)
        {
            return typeof(IFlowNodeData).IsAssignableFrom(type);
        }
    }
}