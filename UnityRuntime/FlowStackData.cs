using System.Collections.Generic;

namespace Flow
{
    [System.Serializable]
    public class FlowStackData
    {
        public string GUID;
        public List<string> Nodes = new List<string>();
    }
}