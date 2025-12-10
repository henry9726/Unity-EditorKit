using System;
using System.Collections.Generic;
using UnityEngine;

namespace Henry.EditorKit
{
    using Component;

    [Serializable]
    public class InstanceStore
    {
        [SerializeField] List<Data> components = new();

        public List<Data> Components => components;

        public void SetData(List<Data> data)
        {
            components.Clear();
            components.AddRange(data);
        }

        public void RemoveData(Data data)
        {
            components.Remove(data);
        }

        public void RemoveAllData()
        {
            foreach (var item in components)
            {
                var comp = item.Component;
                comp.OnDisable();
                
                UnityEngine.Object.DestroyImmediate(item.TargetSO);
            }
            components.Clear();
        }

        public void AddData(Data data)
        {
            components.Add(data);
        }

        public static List<Data> InstanceFromRecord(IReadOnlyList<Record> records, IReadOnlyDictionary<string, Info> infoDict)
        {
            var result = new List<Data>();

            for (int i = 0; i < records.Count; i++)
            {
                var item = records[i];

                if (infoDict.TryGetValue(item.compTypeFullName, out var info) is false)
                {
                    Debug.LogError($"Failed to find info for {item.compTypeFullName}, at records index {i}");
                    continue;
                }

                var componentInstance = CreateComponent(item.compTypeFullName);

                if (componentInstance == null)
                {
                    Debug.LogError($"Failed to create instance of {item.compTypeFullName}, at records index {i}");
                    continue;
                }

                var data = new Data(componentInstance, info, item);

                data.Component.OnInstance();

                result.Add(data);
            }

            return result;
        }

        public static List<Data> InstanceFromInfo(IReadOnlyList<Info> infos)
        {
            var result = new List<Data>();

            for (int i = 0; i < infos.Count; i++)
            {
                var info = infos[i];
                var componentInstance = CreateComponent(info.TypeFullName);

                if (componentInstance == null)
                {
                    Debug.LogError($"Failed to create instance of {info.TypeFullName}, at infos index {i}");
                    continue;
                }

                var record = new Record(info.TypeFullName);
                var data = new Data(componentInstance, info, record);

                data.Component.OnInstance();

                result.Add(data);
            }

            return result;
        }

        public static ScriptableObject CreateComponent(string compTypeFullName)
        {
            var type = Type.GetType(compTypeFullName);

            if (typeof(ScriptableObject).IsAssignableFrom(type) is false)
            {
                Debug.LogError($"Failed to create instance of {type.FullName}, type is not ScriptableObject");
                return null;
            }

            if (typeof(IComponent).IsAssignableFrom(type) is false)
            {
                Debug.LogError($"Failed to create instance of {type.FullName}, type is not IComponent");
                return null;
            }

            var instance = ScriptableObject.CreateInstance(type);
            instance.hideFlags = HideFlags.DontSaveInEditor;
            
            return instance;
        }
    }
}