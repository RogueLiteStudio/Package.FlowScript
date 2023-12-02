using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flow
{
    public class FlowGraphView : GraphView
    {
        public FlowGraphEditor GraphEditor;
        public string SubGraphGUID;

        public Vector2 MousePosition { get; private set; }
        public Vector2 GraphMousePosition => contentViewContainer.WorldToLocal(MousePosition);
        protected override bool canPaste => GraphPasteUtil.CheckPaste(GraphEditor);
        public FlowGraphView()
        {
            style.flexGrow = 1;
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(0.1f, 5f);

            //注册鼠标事件，获取最后鼠标的位置
            RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent);
        }

        // ！！！！狗日的没有默认行为，必须重写！！！！
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return
                ports
                    .ToList()
                    .Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node)
                    .ToList();
        }
        public HashSet<GraphElement> CollectSelectedCopyableGraphElements()
        {
            HashSet<GraphElement> copyables = new HashSet<GraphElement>();
            var elements = selection.OfType<GraphElement>();
            CollectCopyableGraphElements(elements, copyables);
            return copyables;
        }
        private void OnMouseMoveEvent(IMouseEvent evt)
        {
            MousePosition = evt.mousePosition;
        }
    }
}