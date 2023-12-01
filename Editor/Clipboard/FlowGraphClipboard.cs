using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Flow
{
    public class FlowGraphClipboard : ScriptableSingleton<FlowGraphClipboard>
    {
        [SerializeField]
        private List<FlowGraphCopyData> copyDatas = new List<FlowGraphCopyData>();

        public FlowGraphCopyData GetCopyData(FlowGraph graph)
        {
            if (graph == null)
            {
                return null;
            }
            var monoScript = MonoScript.FromScriptableObject(graph);
            return copyDatas.Find(it => it.GraphScript == monoScript);
        }

        public void SetCopy(FlowGraphCopyData data)
        {
            copyDatas.RemoveAll(it => it.GraphScript == data.GraphScript);
            copyDatas.Add(data);
        }
    }
}
