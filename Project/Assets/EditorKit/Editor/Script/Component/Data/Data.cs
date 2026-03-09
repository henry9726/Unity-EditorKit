using System;
using UnityEngine;

namespace Henry.EditorKit.Component
{
    [Serializable]
    public class Data : IDisposable
    {
        [SerializeField] ScriptableObject target;
        [SerializeField] Info info;
        [SerializeField] Record record;
        [SerializeField] string guid;

        bool isDisposed;

        IComponent component;

        public IComponent Component
        {
            get
            {
                if (component == null)
                {
                    component = target as IComponent;
                }
                return component;
            }
        }

        public Info Info => info;

        public Record Record => record;

        public string ID => guid;

        public Data(ScriptableObject target, Info info, Record record)
        {
            this.target = target;
            this.info = info;
            this.record = record;
            guid = Guid.NewGuid().ToString();
        }

        public void SetInfo(Info info)
        {
            this.info = info;
        }

        public void Dispose()
        {
            if (isDisposed) return;

            if (target != null)
            {
                UnityEngine.Object.DestroyImmediate(target);
            }

            target = null;
            info = null;
            record = null;
            component = null;
            isDisposed = true;
        }
    }
}