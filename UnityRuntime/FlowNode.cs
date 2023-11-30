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
        private SerializationData jsonData;
        [System.NonSerialized]
        private IFlowNodeData data;
        public IFlowNodeData Data
        {
            get
            {
                if (data == null)
                {
                    Deserialize();
                }
                return data;
            }
        }
        public void SetData(IFlowNodeData nodeData)
        {
            data = nodeData;
            OnBeforeSerialize();
        }
        public void Deserialize()
        {
            data = TypeSerializerHelper.Deserialize(jsonData) as IFlowNodeData;
        }

        public void OnAfterDeserialize()
        {
            data = null;
        }
        public void OnBeforeSerialize()
        {
            if(data != null)
                jsonData = TypeSerializerHelper.Serialize(data);
        }
    }
}