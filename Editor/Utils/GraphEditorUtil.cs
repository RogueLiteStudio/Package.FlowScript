using UnityEditor;

namespace Flow
{
    public class GraphEditorUtil
    {
        public static void RegisterUndo(FlowGraph graph, string name)
        {
            Undo.RegisterCompleteObjectUndo(graph, name);
            EditorUtility.SetDirty(graph);
        }

        public static void RegisterUndo(FlowGraphEditor editor, string name)
        {
            Undo.RegisterCompleteObjectUndo(editor, name);
            RegisterUndo(editor.Graph.Owner, name);
        }
    }
}
