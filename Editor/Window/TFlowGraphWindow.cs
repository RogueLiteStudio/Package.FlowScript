using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Flow
{
    public class TFlowGraphWindow<TGraph> : FlowGraphWindow where TGraph : FlowGraph
    {
        private ObjectField graphSelect;
        public TGraph Graph => GraphEditor.Graph.Owner as TGraph;

        protected virtual void OnEnable()
        {
            UnityEditor.Undo.undoRedoPerformed += OnUndoRedo;
        }

        protected virtual void OnDisable()
        {
            UnityEditor.Undo.undoRedoPerformed -= OnUndoRedo;
        }
        protected virtual void OnUndoRedo()
        {
            if (GraphEditor)
            {
                GraphEditor.RefreshView();
            }
        }

        public override void OnOpenGraph(FlowGraph graph)
        {
            base.OnOpenGraph(graph);
            graphSelect?.SetValueWithoutNotify(graph);
        }

        protected override void OnCreateLayOut()
        {
            base.OnCreateLayOut();
            var createBtn = TopToolbar.Q<Button>("createAction");
            if (createBtn == null)
            {
                createBtn = new ToolbarButton(CreateAction) { text = "创建" };
                TopToolbar.Add(createBtn);
            }
            else
            {
                createBtn.clicked += CreateAction;
            }
            graphSelect = TopToolbar.Q<ObjectField>("graphSelect");
            if (graphSelect == null)
            {
                graphSelect = new ObjectField();
            }
            graphSelect.objectType = typeof(TGraph);
            graphSelect.allowSceneObjects = true;

            if (GraphEditor)
            {
                graphSelect.SetValueWithoutNotify(Graph);
            }
            graphSelect.RegisterValueChangedCallback(OnGraphSelectChange);
            TopToolbar.Add(graphSelect);
        }
        protected void CreateAction()
        {
            var graph = FlowGraphProcess.OnGraphCreateAction(typeof(TGraph));
            if (graph != null)
            {
                OpenGraph(graph);
            }
        }

        protected void OnGraphSelectChange(ChangeEvent<UnityEngine.Object> evt)
        {
            var graph = evt.newValue as TGraph;
            if (graph)
            {
                OpenGraph(graph);
            }
        }
    }

}