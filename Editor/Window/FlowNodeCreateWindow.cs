using UnityEngine;

namespace Flow
{
    public class FlowNodeCreateWindow : FlowNodeTypeSelectWindow
    {
        protected override void OnTypeSelect(System.Type type, Vector2 position)
        {
            editor.CreateNode(type, position);
        }
    }
}