using System.Collections.Generic;
namespace Flow
{
    [System.Serializable]
    public class FlowEdgeData
    {
        public string GUID;
        public string FromeNode;
        public int OutPort;
        public string ToNode;
        public int InPort;
    }

    //[System.Serializable]
    //public class FlowLink
    //{
    //    public List<FlowEdgeData> Edges = new List<FlowEdgeData>();
    //
    //    //public bool HasLink(FlowNodeRef from, int fromPort, FlowNodeRef to, int toPort)
    //    //{
    //    //    return Edges.Exists(it => it.From == from && it.FromPort == fromPort && it.To == to && it.ToPort == toPort);
    //    //}
    //    //
    //    //public void AddLink(FlowNodeRef from, int fromPort, FlowNodeRef to, int toPort) 
    //    //{
    //    //    Edges.Add(new FlowEdge { From = from, FromPort = fromPort, To = to, ToPort = toPort});
    //    //}
    //}
}