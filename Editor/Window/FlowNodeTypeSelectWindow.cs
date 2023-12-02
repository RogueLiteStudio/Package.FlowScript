using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flow
{
    internal struct NodeTypeData
    {
        public string Name;
        public System.Type Type;
    }
    internal class NodeTypeTree
    {
        public string Name;
        public List<NodeTypeTree> Children = new List<NodeTypeTree>();
        public List<NodeTypeData> Types = new List<NodeTypeData>();

        public NodeTypeTree GetChild(string name)
        {
            var child = Children.Find(it => it.Name == name);
            if (child == null)
            {
                child = new NodeTypeTree() { Name = name };
                Children.Add(child);
            }
            return child;
        }
    }

    public abstract class FlowNodeTypeSelectWindow : ScriptableObject, ISearchWindowProvider
    {
        [SerializeField]
        protected FlowGraphEditor editor;
        [SerializeField]
        private EditorWindow window;
        private Texture2D icon;

        public static T Create<T>(FlowGraphEditor editor, EditorWindow window) where T : FlowNodeTypeSelectWindow
        {
            var w = CreateInstance<T>();
            w.hideFlags = HideFlags.HideAndDontSave;
            w.editor = editor;
            w.window = window;

            w.icon = new Texture2D(1, 1);
            w.icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            w.icon.Apply();

            return w;
        }

        private void OnDestroy()
        {
            if (icon != null)
            {
                DestroyImmediate(icon);
                icon = null;
            }
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>();
            NodeTypeTree rootTree = new NodeTypeTree() { Name = "选择节点类型" };
            List<string> tmpList = new List<string>();
            foreach (var type in editor.NodeTypes)
            {
                //隐藏节点不允许创建
                if (type.GetCustomAttribute<FlowHiddenInNodeCreateAttribute>() != null)
                    continue;
                var dpName = type.GetCustomAttribute<FlowNodeNameAttribute>(false);
                string name = dpName == null ? type.Name : string.Format("{0}({1})", dpName.Name, type.Name);
                tmpList.Clear();
                var catalogType = type;
                while (catalogType != null)
                {
                    var catalog = catalogType.GetCustomAttribute<FlowNodeCatalogAttribute>(false);
                    if (catalog != null && !string.IsNullOrEmpty(catalog.Name))
                    {
                        tmpList.Add(catalog.Name);
                    }
                    catalogType = catalogType.BaseType;
                }
                var root = rootTree;
                for (int i=tmpList.Count-1; i>=0; --i)
                {
                    root = rootTree.GetChild(tmpList[i]);
                }
                root.Types.Add(new NodeTypeData { Name = name, Type = type });
            }
            BuildTree(rootTree, 0, tree);
            return tree;
        }

        private void BuildTree(NodeTypeTree tree, int level, List<SearchTreeEntry> entries)
        {
            entries.Add(new SearchTreeGroupEntry(new GUIContent(tree.Name), level));

            var sortChildren = tree.Children.OrderBy(it => it.Name);
            foreach (var child in sortChildren)
            {
                BuildTree(child, level + 1, entries);
            }

            var sortTypes = tree.Types.OrderBy(it => it.Name);
            foreach (var t in sortTypes)
            {
                entries.Add(new SearchTreeEntry(new GUIContent(t.Name, icon))
                {
                    level = level + 1,
                    userData = t.Type
                });
            }
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var windowRoot = window.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - window.position.position);
            var graphMousePosition = editor.View.contentViewContainer.WorldToLocal(windowMousePosition);
            OnTypeSelect((System.Type)searchTreeEntry.userData, graphMousePosition);
            return true;
        }

        protected virtual bool CheckType(System.Type type)
        {
            return editor.Graph.AllowStageNode || !typeof(IFlowStage).IsAssignableFrom(type);
        }

        protected abstract void OnTypeSelect(System.Type type, Vector2 position);
    }
}