using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Henry.EditorKit
{
    using Component;

    [Serializable]
    public class PinnedPanel : ISubPanel
    {
        StyleSheet style;

        // Initialize fields
        GUIStyle dotMenuBtnStyle;
        GUIContent cornerBtnGuiContent;
        GUILayoutOption emptyMinHeight;

        // Data fields
        [SerializeField] List<Data> comps;
        Vector2 scrollPosition = Vector2.zero;

        Action requestLoadPresetComps;

        public event Action<Data> RequestUnpinCompNotify;
        public event Action<Data> RequestPopupCompNotify;
        public event Action RequestUnpinAllCompNotify;

        public void Setup(Action requestLoadPresetComps)
        {
            this.requestLoadPresetComps = requestLoadPresetComps;
        }

        public void SetPinnedComps(List<Data> comps)
        {
            this.comps = comps;
        }

        void ISubPanel.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Unpin All Components"), false, UnpinAll);
            menu.AddSeparator(string.Empty);
        }

        void ISubPanel.OnEnable()
        {
            emptyMinHeight ??= GUILayout.MinHeight(0);

            foreach (var el in comps)
            {
                if (el.Component != null)
                {
                    el.Component.OnEnable();
                }
            }
        }

        void ISubPanel.OnGUI(Rect rect)
        {
            ValidateStyles();

            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = view.scrollPosition;
                if (comps.Count == 0)
                {
                    DrawEmptyCompHint();
                }
                else
                {
                    DrawComponentCard(rect);
                    GUILayout.FlexibleSpace();
                }
            }
        }

        void ValidateStyles()
        {
            if (style == null)
            {
                style = StyleSheet.Instance;
            }
            if (dotMenuBtnStyle == null)
            {
                dotMenuBtnStyle = new("SearchModeFilter") { stretchWidth = false };
                dotMenuBtnStyle.fixedHeight = 20;
                dotMenuBtnStyle.margin = new(0, 0, 2, 0);
                dotMenuBtnStyle.padding = new(3, 3, 0, 0);
            }
            if (cornerBtnGuiContent == null)
            {
                cornerBtnGuiContent = EditorGUIUtility.IconContent("_Menu");
            }
        }

        void UnpinAll()
        {
            RequestUnpinAllCompNotify?.Invoke();
        }

        void DrawEmptyCompHint()
        {
            EditorGUILayout.HelpBox("No pinned components\nYou can pin components from the [Browse] panel", MessageType.Info);
            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                if (GUILayout.Button("Load preset components"))
                {
                    requestLoadPresetComps?.Invoke();
                }
            }
        }

        void DrawComponentCard(Rect rect)
        {
            // 扣除元件容器的 Padding
            rect.width -= 8;

            var compsCache = comps;
            for (int i = 0; i < compsCache.Count; i++)
            {
                var el = compsCache[i];
                using (new EditorGUILayout.VerticalScope(style.Block))
                {
                    var compRecord = comps[i].Record;
                    var isExpanded = DrawCustomFoldoutHeader(el, compRecord.isExpanded);

                    if (isExpanded)
                    {
                        try
                        {
                            el.Component.OnGUI(rect);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Component {el.Info.Config.Name} failed to draw: {e.Message}");
                            isExpanded = false;
                        }
                    }

                    compRecord.SetExpanded(isExpanded);
                }
            }
        }

        bool DrawCustomFoldoutHeader(Data data, bool isExpanded)
        {
            GUIStyle headerStyle = isExpanded ? style.ExpandedFoldoutHeaderStyle : style.CollapsedFoldoutHeaderStyle;
            string arrowDirectionSymbol = isExpanded ? "▾" : "▸";
            string displayText = $"{arrowDirectionSymbol} {data.Info.Config.Name}";

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(displayText, headerStyle, GUILayout.ExpandWidth(true)))
                {
                    isExpanded = !isExpanded;
                }

                if (GUILayout.Button(cornerBtnGuiContent, dotMenuBtnStyle))
                {
                    ShowComponentMenu(data);
                }
            }

            return isExpanded;
        }

        void ShowComponentMenu(Data data)
        {
            GenericMenu menu = new();
            menu.AddItem(new GUIContent("Unpin"), false, () => UnpinHandler(data));
            menu.AddItem(new GUIContent("Popup"), false, () => PopupHandler(data));
            menu.ShowAsContext();

            void UnpinHandler(Data data)
            {
                RequestUnpinCompNotify?.Invoke(data);
            }

            void PopupHandler(Data data)
            {
                RequestPopupCompNotify?.Invoke(data);
            }
        }
    }
}