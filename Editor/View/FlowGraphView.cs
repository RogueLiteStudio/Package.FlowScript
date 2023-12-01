using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Flow
{
    public class FlowGraphView : GraphView
    {
        public System.Action<DropdownMenuAction> OnCopyAction;
        public System.Action<DropdownMenuAction> OnPasteAction;
        public System.Action<DropdownMenuAction> OnDuplicateAction;
        public System.Func<bool> IsPasteable;


        public FlowGraphView()
        {
            RegisterCallback<ExecuteCommandEvent>(OnExecuteCommand);

            style.flexGrow = 1;
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(0.1f, 5f);
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
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            var items = evt.menu.MenuItems();
            for (int i=items.Count-1; i>=0; --i)
            {
                var item = items[i] as DropdownMenuAction;
                if (item == null)
                    continue;
                switch (item.name)
                {
                    case "Cut":
                        item = new DropdownMenuAction(item.name, CutAction, (DropdownMenuAction a) => (OnCopyAction != null && canCutSelection) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                        break;
                    case "Copy":
                        item = new DropdownMenuAction(item.name, CopyAction, (DropdownMenuAction a) => (OnCopyAction != null && canCopySelection) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                        break;
                    case "Paste":
                        item = new DropdownMenuAction(item.name, PasteAction, (DropdownMenuAction a) => (OnPasteAction != null && IsPasteable != null && IsPasteable()) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                        break;
                    case "Duplicate":
                        item = new DropdownMenuAction(item.name, DuplicateAction, (DropdownMenuAction a) => (OnDuplicateAction != null  && canDuplicateSelection) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                        break;
                }
                items[i] = item;
            }
        }

        private void CutAction(DropdownMenuAction action)
        {
            if (OnCopyAction != null)
            {
                OnCopyAction(action);
                DeleteSelectionOperation("cut", AskUser.DontAskUser);
            }
        }
        private void CopyAction(DropdownMenuAction action)
        {
            OnCopyAction?.Invoke(action);
        }
        private void PasteAction(DropdownMenuAction action)
        {
            OnPasteAction?.Invoke(action);
        }
        private void DuplicateAction(DropdownMenuAction action)
        {
            OnDuplicateAction?.Invoke(action);
        }

        protected virtual void OnExecuteCommand(ExecuteCommandEvent evt)
        {
            if (panel.GetCapturingElement(PointerId.mousePointerId) == null)
            {
                if (evt.commandName == "SoftDelete")
                {
                    evt.StopImmediatePropagation();
                }
            }
        }

        //GraphView内置的复制粘贴依赖剪切板功能，不能实现不同类型的Graph互不影响的复制粘贴
        //参考
        public HashSet<GraphElement> CollectSelectedCopyableGraphElements()
        {
            HashSet<GraphElement> copyables = new HashSet<GraphElement>();
            var elements = selection.OfType<GraphElement>();
            CollectCopyableGraphElements(elements, copyables);
            return copyables;
        }
    }
}