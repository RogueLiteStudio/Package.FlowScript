using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Flow
{
    public class FlowGraphClipboard : ScriptableSingleton<FlowGraphClipboard>
    {
        [SerializeField]
        private List<FlowGraphCopyData> copyDatas = new List<FlowGraphCopyData>();

        public FlowGraphCopyData GetCopyData(FlowGraph graph, string key)
        {
            if (graph == null)
            {
                return null;
            }

            var monoScript = MonoScript.FromScriptableObject(graph);
            return copyDatas.Find(it => it.GraphScript == monoScript && (key == string.Empty || key == it.Key));
        }

        public void SetCopy(FlowGraphCopyData data)
        {
            copyDatas.Add(data);
            bool remove = false;
            for (int i=copyDatas.Count-2; i>=0; --i)
            {
                if (copyDatas[i].GraphScript == data.GraphScript)
                {
                    if (remove)
                    {
                        copyDatas.RemoveAt(i);
                    }
                    else
                    {
                        remove = true;
                    }
                }
            }
        }

        public void Remove(string key)
        {
            copyDatas.RemoveAll(it => it.Key == key);
        }
    }
}
