using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Flow
{
    public class FlowGraphWindow : UnityEditor.EditorWindow
    {
        public FlowGraphEditor GraphEditor;
        [UnityEditor.Callbacks.OnOpenAsset(0)]
        internal static bool OnGraphOpened(int instanceID, int line)
        {
            var asset = UnityEditor.EditorUtility.InstanceIDToObject(instanceID) as FlowGraph;
            return asset != null && OpenGraph(asset);
        }

        public static bool OpenGraph(FlowGraph graph)
        {
            var windowType = FlowGraphProcess.GetEditorWindowType(graph);
            if (windowType != null)
            {
                var window = GetWindow(windowType, false, null) as FlowGraphWindow;
                window.OnOpenGraph(graph);
                return true;
            }
            return false;
        }

        public VisualElement GraphViewRoot { get; protected set; }
        public VisualElement InspectorView { get; protected set; }

        public Toolbar TopToolbar { get; protected set; }

        protected virtual string UXMLPath => null;

        public void CreateGUI()
        {
            if (!string.IsNullOrEmpty(UXMLPath))
            {
                var visualTree = UnityEngine.Resources.Load<VisualTreeAsset>(UXMLPath);
                visualTree.CloneTree(rootVisualElement);
                TopToolbar = rootVisualElement.Q<Toolbar>("toolbar");
                GraphViewRoot = rootVisualElement.Q<VisualElement>("graphViewRoot");
            }
            OnCreateLayOut();
            if (GraphEditor)
            {
                GraphEditor.RefreshView();
            }
        }

        public virtual FlowGraphEditor CreateGraphEditor(FlowGraph graph)
        {
            FlowGraphEditor graphEditor = CreateInstance<FlowGraphEditor>();
            graphEditor.Graph = graph.SubGraphs[0];
            graphEditor.window = this;
            graphEditor.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
            return graphEditor;
        }

        public virtual void OnOpenGraph(FlowGraph graph)
        {
            if (GraphEditor == null)
            {
                GraphEditor = CreateGraphEditor(graph);
                GraphEditor.RefreshView();
            }
            else
            {
                GraphEditor.Graph = graph.SubGraphs[0];
                GraphEditor.RefreshView();
            }
        }
        protected void OnDestroy()
        {
            if (GraphEditor != null)
            {
                DestroyImmediate(GraphEditor);
                GraphEditor = null;
            }
        }

        protected virtual void OnCreateLayOut()
        {
            if (TopToolbar == null)
            {
                rootVisualElement.Add(TopToolbar = new Toolbar());
            }
            if (GraphViewRoot == null)
            {
                var split = new TwoPaneSplitView(1, 200, TwoPaneSplitViewOrientation.Horizontal);
                rootVisualElement.Add(split);
                split.Add(GraphViewRoot = new VisualElement());
                split.Add(InspectorView = new VisualElement());
            }
        }
    }
}
