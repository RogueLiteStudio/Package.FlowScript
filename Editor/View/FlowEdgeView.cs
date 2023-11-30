using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Flow
{
    public class FlowEdgeView : Edge
    {
        readonly static string edgeStyle = "FlowGraphStyles/EdgeView";
        public FlowEdgeView()
        {
            styleSheets.Add(UnityEngine.Resources.Load<StyleSheet>(edgeStyle));
        }
    }
}
