using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Flow
{
    public class FlowGraphView : GraphView
    {
        public FlowGraphView()
        {
            style.flexGrow = 1;
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(0.1f, 5f);
        }

    }


}