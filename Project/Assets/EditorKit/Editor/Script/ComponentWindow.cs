using UnityEditor;
using UnityEngine;

namespace Henry.EditorKit
{
    using Henry.EditorKit.Component;

    sealed internal class ComponentWindow : EditorWindow, IHasCustomMenu
    {
        public static ComponentWindow Create(Data data)
        {
            ComponentWindow[] windows = Resources.FindObjectsOfTypeAll<ComponentWindow>();
            ComponentWindow tempLatestWindow = windows.Length > 0 ? windows[0] : null;

            ComponentWindow window = CreateInstance<ComponentWindow>();
            string compName = data.Info.Config.Name;
            window.titleContent = new GUIContent(compName);
            window.Setup(data);
            window.Show(true);
            window.Focus();

            if (tempLatestWindow != null)
            {
                var rect = tempLatestWindow.position;
                rect.position += new Vector2(20f, 20f);
                window.position = rect;
            }

            return window;
        }

        [SerializeField] string componentTypeFullName;
        [SerializeField] string persistentContent;
        [SerializeField] bool useUpdateLoop = false;
        [SerializeField] Data compData;

        IComponent component;
        bool isNeedReSetup = false;
        Vector2 scrollPosition = Vector2.zero;

        public void Setup(Data compData)
        {
            this.compData = compData;
            componentTypeFullName = compData.Info.TypeFullName;
            persistentContent = string.Empty;
            component = compData.Component;
        }

        bool TryReCreateComponent()
        {
            if (string.IsNullOrEmpty(componentTypeFullName))
            {
                return false;
            }

            component = compData.Component;

            if (component == null)
            {
                return false;
            }
            else
            {
                component.OnInstance();
                component.OnEnable();
            }

            isNeedReSetup = false;
            return true;
        }

        void OnDisable()
        {
            isNeedReSetup = true;
        }

        void OnDestroy()
        {
            compData.Dispose();
            compData = null;
        }

        void Update()
        {
            if (useUpdateLoop)
            {
                Repaint();
            }
        }

        void OnGUI()
        {
            if (isNeedReSetup && TryReCreateComponent() is false)
            {
                Close();
                return;
            }

            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = view.scrollPosition;
                component.OnGUI(position);
            }
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Refresh Mode/Event Based (Standard)"), !useUpdateLoop, () =>
            {
                useUpdateLoop = false;
            });
            menu.AddItem(new GUIContent("Refresh Mode/Continuous (Update Loop)"), useUpdateLoop, () =>
            {
                useUpdateLoop = true;
            });
        }
    }
}