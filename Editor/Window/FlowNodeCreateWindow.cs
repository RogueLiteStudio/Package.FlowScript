using System;
using UnityEngine;

namespace Flow
{
    public class FlowNodeCreateWindow : FlowNodeTypeSelectWindow
    {
        protected override void OnTypeSelect(System.Type type, Vector2 position)
        {
            editor.CreateNode(type, position);
        }

        protected override bool CheckType(Type type)
        {
            //入口节点不允许创建
            if (typeof(IFlowEntry).IsAssignableFrom(type))
                return false;
            return base.CheckType(type);
        }
    }
}