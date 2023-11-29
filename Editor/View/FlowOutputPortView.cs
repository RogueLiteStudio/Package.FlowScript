using System;
using UnityEditor.Experimental.GraphView;

namespace Flow
{
    public class FlowOutputPortView : Port
    {
        private FlowSubGraph owner;
        private string nodeGUID;
        private int index;
        private static Type defaultPortType => typeof(bool);
        protected FlowOutputPortView()
            : base(Orientation.Horizontal, Direction.Output, Capacity.Single, defaultPortType)
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