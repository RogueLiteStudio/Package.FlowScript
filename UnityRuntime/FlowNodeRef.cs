using System;
using UnityEngine;

namespace Flow
{
    [System.Serializable]
    public struct FlowNodeRef : IEquatable<FlowNodeRef>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private string nodeGUID;
        [SerializeField]
        private FlowGraph graph;

        private FlowNode node;

        public string GUID => nodeGUID;

        public FlowNode Node
        {
            get
            {
                if (node == null && graph)
                {
                    node = graph.FindNode(nodeGUID);
                }
                return node;
            }
        }

        public readonly static FlowNodeRef None = new FlowNodeRef();

        public static FlowNodeRef CreateNodeRef(FlowGraph graph, string guid)
        {
            return new FlowNodeRef { graph = graph, nodeGUID = guid };
        }

        public readonly bool Equals(FlowNodeRef other)
        {
            return graph == other.graph && nodeGUID == other.nodeGUID;
        }

        public readonly void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            node = null;
        }

        public static implicit operator bool(FlowNodeRef exists)
        {
            return exists.Node != null;
        }
    }
}