using UnityEngine;

namespace Flow
{
    [System.Serializable]
    public class FlowNode : ISerializationCallbackReceiver
    {
        public string GUID;
        public string Name;
        public string Comment;
        [SerializeField]
        [HideInInspector]
        private JsonElement jsonData;
        [System.NonSerialized]
        private IFlowNodeData data;
        public IFlowNodeData Data => data;
        public void SetData(IFlowNodeData nodeData)
        {
            data = nodeData;
            OnBeforeSerialize();
        }
        public void OnAfterDeserialize()
        {
            data = JsonSerializer.DeserializeNode(jsonData); ;
        }

        public void OnBeforeSerialize()
        {
            jsonData = JsonSerializer.SerializeNode(data);
        }
    }
}