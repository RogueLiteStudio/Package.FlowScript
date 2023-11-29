using UnityEngine;

namespace Flow
{
    public class FlowGraphEditor : ScriptableObject
    {
        public FlowSubGraph Graph;
        public int SubGraphIndex;

        public FlowGraphView View;

        public void OnUndoRedo()
        {

        }

        public virtual void OnInspector()
        {

        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        private void OnDestroy()
        {
            
        }
    }
}