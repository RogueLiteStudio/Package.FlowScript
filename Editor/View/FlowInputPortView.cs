using System;
using UnityEditor.Experimental.GraphView;

namespace Flow
{
    public class FlowInputPortView : Port
    {
        private FlowSubGraph owner;
        private string nodeGUID;
        private int index;
        private static Type defaultPortType => typeof(bool);
        protected FlowInputPortView() 
            : base(Orientation.Horizontal, Direction.Input, Capacity.Multi, defaultPortType)
        {
        }

        public void SetOwner(FlowSubGraph subGraph, FlowNode node, int index)
        {
            owner = subGraph;
            nodeGUID = node.GUID;
            this.index = index;
        }

    }
}