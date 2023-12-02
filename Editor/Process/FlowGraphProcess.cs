using System;
using System.Collections.Generic;
using System.Reflection;

namespace Flow
{
    public static class FlowGraphProcess
    {
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
                                if (process.ContainsKey(attribute.GraphType))
                                {
                                    process.Remove(attribute.GraphType);
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
        }
    }
}
