using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

namespace Flow
{
    internal class EdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphViewChange m_GraphViewChange;
        private List<Edge> m_EdgesToCreate;
        private List<GraphElement> m_EdgesToDelete;

        public EdgeConnectorListener()
        {
            m_EdgesToCreate = new List<Edge>();
            m_EdgesToDelete = new List<GraphElement>();
            m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            m_EdgesToCreate.Clear();
            m_EdgesToCreate.Add(edge);
            m_EdgesToDelete.Clear();
            if (edge.input.capacity == Capacity.Single)
            {
                foreach (Edge connection in edge.input.connections)
                {
                    if (connection != edge)
                        m_EdgesToDelete.Add(connection);
                }
            }
            if (edge.output.capacity == Capacity.Single)
            {
                foreach (Edge connection in edge.output.connections)
                {
                    if (connection != edge)
                        m_EdgesToDelete.Add(connection);
                }
            }
            if (m_EdgesToDelete.Count > 0)
                graphView.DeleteElements(m_EdgesToDelete);
            List<Edge> edgesToCreate = m_EdgesToCreate;
            if (graphView.graphViewChanged != null)
                edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
            foreach (Edge newEdge in edgesToCreate)
            {
                graphView.AddElement(newEdge);
                edge.input.Connect(newEdge);
                edge.output.Connect(newEdge);
            }
        }
    }
}
