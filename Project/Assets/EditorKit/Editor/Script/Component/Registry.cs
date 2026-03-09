using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Henry.EditorKit
{
    using Henry.EditorKit.Component;
    using CompInfo = Component.Info;

    sealed internal class ComponentRegistry
    {
        readonly static List<CompInfo> list = new();
        readonly static Dictionary<string, CompInfo> infoDict = new();

        public static IReadOnlyList<CompInfo> List
        {
            get
            {
                if (list.Count == 0)
                {
                    ScanComponentSource();
                }
                return list;
            }
        }

        public static IReadOnlyDictionary<string, CompInfo> InfoDict
        {
            get
            {
                if (infoDict.Count == 0)
                {
                    ScanComponentSource();
                }
                return infoDict;
            }
        }

        static void ScanComponentSource()
        {
            list.Clear();
            infoDict.Clear();

            string[] guids = AssetDatabase.FindAssets("t:ComponentManifest");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var manifest = AssetDatabase.LoadAssetAtPath<ComponentManifest>(path);

                if (manifest == null) continue;

                var info = new CompInfo(manifest);

                if (info.ComponentType != null)
                {
                    list.Add(info);
                    if (!infoDict.ContainsKey(info.TypeFullName))
                    {
                        infoDict.Add(info.TypeFullName, info);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate ComponentManifest found for type: {info.TypeFullName}. Ignoring asset at {path}");
                    }
                }
            }
        }
    }
}