using UnityEngine;

namespace Henry.EditorKit
{
    public interface IComponent
    {
        void OnInstance() { }

        void OnEnable() { }

        void OnDisable() { }

        void OnGUI(Rect rect);

        string CollectPersistentContent() => string.Empty;

        void RestorePersistentContent(string content) { }
    }
}