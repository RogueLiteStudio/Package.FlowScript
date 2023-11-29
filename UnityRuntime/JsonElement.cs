using System;
using UnityEditor;
namespace Flow
{

    [Serializable]
    public struct JsonElement
    {
        public string Type;
        public string JsonDatas;

        public override string ToString()
        {
            return "type: " + Type + " | JSON: " + JsonDatas;
        }
    }

    public static class JsonSerializer
    {
        public static JsonElement Serialize(object obj)
        {
            JsonElement elem = new JsonElement();

            elem.Type = obj.GetType().AssemblyQualifiedName;
#if UNITY_EDITOR
            elem.JsonDatas = EditorJsonUtility.ToJson(obj);
#else
            elem.JsonDatas = UnityEngine.JsonUtility.ToJson(obj);
#endif

            return elem;
        }

        public static T Deserialize<T>(JsonElement e)
        {
            if (typeof(T) != Type.GetType(e.Type))
                throw new ArgumentException("Deserializing type is not the same than Json element type");

            var obj = Activator.CreateInstance<T>();
#if UNITY_EDITOR
            EditorJsonUtility.FromJsonOverwrite(e.JsonDatas, obj);
#else
            UnityEngine.JsonUtility.FromJsonOverwrite(e.JsonDatas, obj);
#endif

            return obj;
        }

        public static JsonElement SerializeNode(IFlowNodeData node)
        {
            return Serialize(node);
        }

        public static IFlowNodeData DeserializeNode(JsonElement e)
        {
            try
            {
                var baseNodeType = Type.GetType(e.Type);

                if (e.JsonDatas == null)
                    return null;

                var node = Activator.CreateInstance(baseNodeType) as IFlowNodeData;
#if UNITY_EDITOR
                EditorJsonUtility.FromJsonOverwrite(e.JsonDatas, node);
#else
                UnityEngine.JsonUtility.FromJsonOverwrite(e.JsonDatas, node);
#endif
                return node;
            }
            catch
            {
                return null;
            }
        }
    }
}