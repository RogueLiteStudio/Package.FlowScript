using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Reflection;

namespace Flow
{
    public class FlowGraphEditor : ScriptableObject
    {
        public FlowSubGraph Graph;
        public FlowGraphView View;
        public EditorWindow window;
        private FlowNodeCreateWindow nodeCreateWindow;
        public List<FlowNodeRef> SelectNodes = new List<FlowNodeRef>();

        private readonly List<IFlowNodeView> nodeViews = new List<IFlowNodeView>();
        private readonly List<Edge> edgeViews = new List<Edge>();

        protected IFlowNodeView FindView(string guid)
        {
            return nodeViews.Find(it => it.GUID == guid);
        }

        private List<System.Type> nodeTypes;

        public IReadOnlyList<System.Type> NodeTypes
        {
            get
            {
                if (nodeTypes == null)
                {
                    nodeTypes = new List<System.Type>();
                    foreach (var assemble in System.AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (var type in assemble.GetTypes())
                        {
                            if (type.IsInterface || type.IsAbstract)
                                continue;
                            if (Graph.Owner.CheckIsValidNodeType(type))
                            {
                                nodeTypes.Add(type);
                            }
                        }
                    }
                }
                return nodeTypes;
            }
        }

        public FlowNode CreateNode(System.Type type, Vector2 position)
        {
            GraphEditorUtil.RegisterUndo(Graph.Owner, "create node");

            IFlowNodeData nodeData = System.Activator.CreateInstance(type) as IFlowNodeData;
            FlowNode node = GraphCreateUtil.CreateNode(Graph, nodeData, new Rect(position, new Vector2(100, 100)));
            if (node != null)
            {
                var fnn = type.GetCustomAttribute<FlowNodeNameAttribute>(false);
                if (fnn != null && string.IsNullOrEmpty(fnn.Name))
                    node.Name = fnn.Name;
                else
                    node.Name = type.Name;
                if (nodeData is IFlowStackNode)
                {
                    var nodeView = new FlowStackNodeView();
                    View.AddElement(nodeView);
                    nodeView.BindNode(this, node);
                    nodeViews.Add(nodeView);
                }
                else
                {
                    var nodeView = new FlowNodeView();
                    View.AddElement(nodeView);
                    nodeView.BindNode(this, node);
                    nodeViews.Add(nodeView);
                }
            }
            return node;
        }

        public void RefreshView()
        {
            if (View == null)
            {
                if (!nodeCreateWindow)
                {
                    nodeCreateWindow = FlowNodeTypeSelectWindow.Create<FlowNodeCreateWindow>(this, window);
                }
                View = new FlowGraphView
                {
                    viewTransformChanged = ViewTransformChangedCallback,
                    graphViewChanged = GraphViewChangedCallback,
                    nodeCreationRequest = (c)=>SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), nodeCreateWindow),
                };
                window.rootVisualElement.Add(View);
                View.StretchToParentSize();
            }
            //删除
            for (int i=nodeViews.Count-1; i>=0; --i)
            {
                var nodeView = nodeViews[i];
                if (!Graph.Nodes.Exists(it=>it.GUID == nodeView.GUID))
                {
                    View.RemoveElement(nodeView as Node);
                    nodeViews.RemoveAt(i);
                }
                else
                {
                    (nodeView as Node).selected = SelectNodes.Exists(it => it.GUID == nodeView.GUID);
                }
            }
            //TODO:处理 stackview
            //for (int i = stackViews.Count - 1; i >= 0; --i)
            //{
            //    var nodeView = stackViews[i];
            //    if (!Graph.Nodes.Exists(it => it.GUID == nodeView.viewDataKey))
            //    {
            //        View.RemoveElement(nodeView);
            //        stackViews.RemoveAt(i);
            //    }
            //    else
            //    {
            //        nodeView.selected = SelectNodes.Exists(it => it.GUID == nodeView.viewDataKey);
            //    }
            //}
            //创建
            foreach (var nodeRef in Graph.Nodes)
            {
                var nodeView = nodeViews.Find(it => it.GUID == nodeRef.GUID);
                if (nodeView == null)
                {
                    if (nodeRef.Node.Data is IFlowStackNode)
                    {
                        var stack = new FlowStackNodeView();
                        nodeView = stack;
                        View.AddElement(stack);
                        stack.BindNode(this, nodeRef.Node);
                        nodeViews.Add(stack);
                    }
                    else
                    {
                        var node = new FlowNodeView();
                        nodeView = node;
                        View.AddElement(node);
                        node.BindNode(this, nodeRef.Node);
                        nodeViews.Add(node);

                    }
                }
                else
                {
                    nodeView.RefreshNodeView(nodeRef.Node);
                }
                (nodeView as Node).selected = SelectNodes.Exists(it => it.GUID == nodeView.GUID);
            }
            //删除不存在或者不匹配的
            for (int i = edgeViews.Count - 1; i >= 0; --i)
            {
                var edgeView = edgeViews[i];
                var edgeData = Graph.Edges.Find(it => it.GUID == edgeView.viewDataKey);

                if (edgeData == null)
                {
                    View.RemoveElement(edgeView);
                    edgeViews.RemoveAt(i);
                    continue;
                }
                if (edgeView.input is not FlowPortView fromNode || edgeView.output is not FlowPortView toNode)
                {
                    View.RemoveElement(edgeView);
                    edgeViews.RemoveAt(i);
                    continue;
                }
                if (fromNode.NodeGUID != edgeData.FromeNode || fromNode.Index != edgeData.InPort )
                {
                    View.RemoveElement(edgeView);
                    edgeViews.RemoveAt(i);
                    continue;
                }
                if (toNode.NodeGUID != edgeData.ToNode || toNode.Index != edgeData.OutPort)
                {
                    View.RemoveElement(edgeView);
                    edgeViews.RemoveAt(i);
                    continue;
                }
            }
            //创建不存在的
            foreach (var edgeData in Graph.Edges)
            {
                var edgeView = edgeViews.Find(it => it.viewDataKey == edgeData.GUID);
                if (edgeView == null)
                {
                    var fromNode = FindView(edgeData.FromeNode);
                    var toNode = FindView(edgeData.ToNode);
                    if (fromNode == null || toNode == null)
                        continue;
                    var fromPort = fromNode.OutputPorts[edgeData.OutPort];
                    var toPort = toNode.InputPorts[edgeData.InPort];
                    if (fromPort == null || toPort == null)
                        continue;
                    edgeView = new Edge()
                    {
                        input = toPort,
                        output = fromPort,
                        viewDataKey = edgeData.GUID,
                    };
                    View.AddElement(edgeView);
                    edgeViews.Add(edgeView);
                }
            }
        }

        public virtual void OnInspector()
        {

        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        private GraphViewChange GraphViewChangedCallback(GraphViewChange changes)
        {
            if (changes.elementsToRemove != null)
            {
                changes.elementsToRemove.RemoveAll(it =>
                {
                    if (it is FlowNodeView nodeView)
                    {
                        var node = Graph.Owner.FindNode(nodeView.viewDataKey);
                        if (node != null && node.Data is IFlowEntry)
                            return true;
                    }
                    return false;
                });
                if (changes.elementsToRemove.Count > 0)
                {
                    GraphEditorUtil.RegisterUndo(Graph.Owner, "remove element");
                    foreach (var ele in changes.elementsToRemove)
                    {
                        switch (ele)
                        {
                            case null:
                                break;
                            case Edge edge:
                                edgeViews.RemoveAll(it=>it.viewDataKey == edge.viewDataKey);
                                GraphDeleteUtil.RemoveEdge(Graph, edge.viewDataKey);
                                break;
                            case IFlowNodeView nodeView:
                                nodeViews.RemoveAll(it => it.GUID == nodeView.GUID);
                                GraphDeleteUtil.RemoveNode(Graph, nodeView.GUID);
                                break;
                        }
                    }
                }
            }
            if (changes.edgesToCreate != null)
            {
                GraphEditorUtil.RegisterUndo(Graph.Owner, "create edge");
                foreach (var edge in changes.edgesToCreate)
                {
                    var fromNodeView = edge.output.node as FlowNodeView;
                    var toNodeView = edge.input.node as FlowNodeView;
                    var fromPort = edge.output as FlowPortView;
                    var toPort = edge.input as FlowPortView;

                    var edgeData = GraphCreateUtil.CreateEdge(Graph, fromNodeView.viewDataKey, fromPort.Index, toNodeView.viewDataKey, toPort.Index);
                    edge.viewDataKey = edgeData.GUID;
                    edgeViews.Add(edge);
                }
            }
            return changes;
        }
        private void ViewTransformChangedCallback(GraphView view)
        {
            Graph.Position = view.viewTransform.position;
            Graph.Scale = view.viewTransform.scale;
        }

        private void OnEdgeMousEvent(MouseDownEvent e)
        {
            if (e.clickCount == 2)
            {
                var position = e.mousePosition;
                position += new Vector2(-10f, -28);
                Vector2 mousePos = View.ChangeCoordinatesTo(View.contentViewContainer, position);

                //添加Prvite
            }
        }

        private void OnDestroy()
        {
            if (nodeCreateWindow != null)
            {
                DestroyImmediate(nodeCreateWindow);
                nodeCreateWindow = null;
            }
        }
    }
}