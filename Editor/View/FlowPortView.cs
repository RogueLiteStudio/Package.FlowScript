using System;
using UnityEditor.Experimental.GraphView;

namespace Flow
{
    public class FlowPortView : Port
    {
        public FlowGraphEditor GraphEditor;
        public string NodeGUID;
        public int Index;
        public bool IsInput => direction == Direction.Input;
        private static Type defaultPortType => typeof(bool);
        public FlowPortView(bool isInput)
            : base(Orientation.Horizontal, isInput ? Direction.Input : Direction.Output, isInput ? Capacity.Multi : Capacity.Single, defaultPortType)
        {
        }
    }
}