using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    //TODO:暂时不支持
    [System.Serializable]
    public class FlowNodeGroup
    {
        public string GUID;
        public string Name;
        public Rect Position;
        public Color Color = new Color(0, 0, 0, 0.3f);
        public List<string> Nodes = new List<string>();
    }
}