using System;
using System.Collections.Generic;
using System.Reflection;

namespace Flow
{
    public static class FlowGraphProcess
    {
        private static Dictionary<Type, IReadOnlyList<Type>> _nodeTypes = new Dictionary<Type, IReadOnlyList<Type>>();
        private static Dictionary<Type, IFlowGraphProcess> process;
        public static Dictionary<Type, IFlowGraphProcess> Process
        {
            get
            {
                if (process == null)
                {
                    process = new Dictionary<Type, IFlowGraphProcess>();
                    foreach (var assemble in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (var type in assemble.GetTypes())
                        {
                            if (type.IsInterface || type.IsAbstract)
                                continue;
                            if (typeof(IFlowGraphProcess).IsAssignableFrom(type))
                            {
                                var attribute = type.GetCustomAttribute<CustomFlowGraphProcessAttribute>(false);
                                if (attribute != null)
                                {
                                    if (!attribute.GraphType.IsSubclassOf(typeof(FlowGraph)))
                                    {
                                        UnityEngine.Debug.LogError($"{type.Name} 的 CustomFlowGraphProcessAttribute 类型错误，不是 FlowGraph 的子类");
                                    }
                                    else if (process.ContainsKey(attribute.GraphType))
                                    {
                                        process.Remove(attribute.GraphType);
                                    }
                                }
                                process.Add(attribute.GraphType, Activator.CreateInstance(type) as IFlowGraphProcess);
                            }
                        }
                    }
                }
                return process;
            }
        }

        public static Type GetEditorWindowType(FlowGraph graph)
        {
            var type = graph.GetType();
            if (Process.TryGetValue(type, out var proc))
            {
                return proc.EditorWindowType;
            }
            return null;
        }

        public static FlowGraph OnGraphCreateAction(Type graphType)
        {
            if (Process.TryGetValue(graphType, out var proc))
            {
                return proc.OnCreateAction();
            }
            return null;
        }

        public static void OnGraphCreateSubGraph(FlowGraph graph, FlowSubGraph subGraph, FlowSubGraph prarent, FlowNode bindNode)
        {
            var type = graph.GetType();
            if (Process.TryGetValue(type, out var proc))
            {
                proc.OnCreateSubGraph(graph, subGraph, prarent, bindNode);
            }
        }

        public static void OnGraphSave(FlowGraph graph)
        {
            var type = graph.GetType();
            if (Process.TryGetValue(type, out var proc))
            {
                proc.OnSave(graph);
            }
            else
            {
                UnityEditor.AssetDatabase.SaveAssetIfDirty(graph);
            }
        }

        public static IReadOnlyList<Type> GetNodeTypes(FlowGraph graph)
        {
            var type = graph.GetType();
            if (!_nodeTypes.TryGetValue(type, out var tys))
            {
                if (Process.TryGetValue(type, out var proc))
                {
                    var list = new List<Type>();
                    tys = list;
                    _nodeTypes.Add(type, tys);
                    foreach (var assemble in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (var t in assemble.GetTypes())
                        {
                            if (t.IsInterface || t.IsAbstract)
                                continue;
                            if (proc.CheckIsValidNodeType(t))
                            {
                                list.Add(t);
                            }
                        }
                    }
                }
            }
            return tys;
        }
    }
}
