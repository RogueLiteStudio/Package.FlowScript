using UnityEngine;

namespace Flow
{
    [System.Serializable]
    public class FlowNodeViewData
    {
        public string NodeGUID;
        public Rect Position;
        public bool Expanded = true;
    }
}