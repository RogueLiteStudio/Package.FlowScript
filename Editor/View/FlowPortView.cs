using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
namespace Flow
{
    public class FlowPortView : Port
    {
        public FlowGraphEditor GraphEditor;
        public string NodeGUID;
        public int Index;
        public bool IsInput => direction == Direction.Input;

        readonly string styleFile = "FlowGraphStyles/PortView";
        public FlowPortView(bool isInput)
            : base(Orientation.Horizontal, isInput ? Direction.Input : Direction.Output, isInput ? Capacity.Multi : Capacity.Single, typeof(float))
        {
            var connectorListener = new EdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<FlowEdgeView>(connectorListener);
            VisualElementExtensions.AddManipulator(this, m_EdgeConnector);
            var styleSheet = UnityEngine.Resources.Load<StyleSheet>(styleFile);
            if (styleSheet)
                styleSheets.Add(styleSheet);

            portName = "";//默认不显示端口名称
            /*
             * visualClass 为设置Port的样式，在uss文件中的 .Port_XXX 中配置
             */

            if (isInput)
            {
                visualClass = "Port_In";
            }
            else
            {
                visualClass = "Port_Out";
            }
        }
    }
}