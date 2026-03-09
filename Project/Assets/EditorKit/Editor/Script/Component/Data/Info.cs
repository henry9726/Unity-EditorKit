using System;
using UnityEngine;

namespace Henry.EditorKit.Component
{
    [Serializable]

    public class Info
    {
        [SerializeField] Config config;
        [SerializeField] string typeFullName;

        Type componentType;

        public Config Config => config;

        public string TypeFullName => typeFullName;

        public Type ComponentType
        {
            get
            {
                if (componentType == null)
                {
                    componentType = Type.GetType(TypeFullName);
                }
                return componentType;
            }
        }

        public Info(ComponentManifest manifest)
        {
            config = manifest.Config;

            if (manifest.Script != null)
            {
                var type = manifest.Script.GetClass();
                if (type != null)
                {
                    componentType = type;
                    typeFullName = type.FullName;
                }
            }
        }
    }
}