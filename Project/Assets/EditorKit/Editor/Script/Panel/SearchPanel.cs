using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Henry.EditorKit
{
    using Henry.EditorKit.Component;

    [Serializable]
    class SearchPanel : ISubPanel
    {
        readonly string searchBarHeaderText = "Search : ";

        IReadOnlyList<Info> componentInfoList;
        bool[] isExpandedList;
        ISearcher searcher;
        StyleSheet style;

        readonly Color gray75 = new(0.75f, 0.75f, 0.75f, 1);

        Texture2D infoIcon;
        Texture2D openIcon;
        Texture2D keepIcon;

        GUIContent infoIconContent;
        GUIContent openIconContent;
        GUIContent keepIconContent;
        GUIStyle labelStyle;
        GUILayoutOption searchBarHeaderWidth;

        [SerializeField] string searchingString = "";
        IReadOnlyList<Info> searchResult = new List<Info>();
        Vector2 scrollPosition = Vector2.zero;

        public event Action<Info> OnRequestPinComp;
        public event Action<Info> OnRequestOpenComp;

        public SearchPanel()
        {
        }

        void ISubPanel.OnEnable()
        {
            if (componentInfoList == null)
            {
                componentInfoList = ComponentRegistry.List;
                searchResult = componentInfoList;
                isExpandedList = new bool[componentInfoList.Count];
            }

            searcher = SearcherManager.GetSearcher();
            style = StyleSheet.Instance;

            infoIcon = EditorGUIUtility.Load(RootConfig.AssetsPath + "/Editor/Icons/info_16dp.png") as Texture2D;
            openIcon = EditorGUIUtility.Load(RootConfig.AssetsPath + "/Editor/Icons/open_in_new_16dp.png") as Texture2D;
            keepIcon = EditorGUIUtility.Load(RootConfig.AssetsPath + "/Editor/Icons/keep_16dp.png") as Texture2D;

            infoIconContent = new GUIContent(infoIcon, "Open help");
            openIconContent = new GUIContent(openIcon, "Open in new window");
            keepIconContent = new GUIContent(keepIcon, "Pin it");
        }

        void ISubPanel.OnGUI(Rect rect)
        {
            ValidateStyles();
            DrawSearchBar();

            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = view.scrollPosition;
                for (int i = 0; i < searchResult.Count; i++)
                {
                    DrawComponentCard(searchResult[i], i);
                }
            }
        }

        void ValidateStyles()
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.label);
            }
            if (searchBarHeaderWidth == null)
            {
                searchBarHeaderWidth = GUILayout.MaxWidth(labelStyle.CalcSize(new GUIContent(searchBarHeaderText)).x);
            }
        }

        void DrawSearchBar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(searchBarHeaderText, searchBarHeaderWidth);
                EditorGUI.BeginChangeCheck();
                searchingString = EditorGUILayout.TextField(string.Empty, searchingString);
                if (EditorGUI.EndChangeCheck())
                {
                    var tempSearchString = searchingString;
                    if (string.IsNullOrEmpty(tempSearchString))
                    {
                        searchResult = componentInfoList;
                    }
                    else
                    {
                        searcher.Search(tempSearchString, result =>
                        {
                            searchResult = result.Select(el => componentInfoList.First(item => item.TypeFullName == el)).ToList();
                        });
                    }
                }
            }
        }

        void DrawComponentCard(Info info, int index)
        {
            var config = info.Config;

            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                var headerText = config.Name;
                var isExpanded = isExpandedList[index];
                var headerStyle = style.ExpandedFoldoutHeaderStyle;
                isExpanded = DrawCustomFoldoutHeader(headerText, isExpanded, headerStyle);

                if (!string.IsNullOrEmpty(config.Description))
                {
                    EditorGUILayout.LabelField(config.Description, EditorStyles.wordWrappedLabel);
                }

                if (isExpanded)
                {

                    Color originalColor = GUI.contentColor;
                    GUI.contentColor = gray75;
                    EditorGUILayout.LabelField($"Author  : {config.Author}", GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField($"Version : {config.Version}", GUILayout.ExpandWidth(false));
                    GUI.contentColor = originalColor;
                }
                isExpandedList[index] = isExpanded;

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    GUI.enabled = !string.IsNullOrEmpty(config.ReadmePath);
                    if (GUILayout.Button(infoIconContent, GUILayout.Width(36), GUILayout.ExpandWidth(false)))
                    {
                        Application.OpenURL(config.ReadmePath);
                    }
                    GUI.enabled = true;

                    if (GUILayout.Button(openIconContent, GUILayout.Width(36), GUILayout.ExpandWidth(false)))
                    {
                        OnRequestOpenComp?.Invoke(info);
                    }

                    if (GUILayout.Button(keepIconContent, GUILayout.Width(36), GUILayout.ExpandWidth(false)))
                    {
                        OnRequestPinComp?.Invoke(info);
                    }
                }
            }
        }

        bool DrawCustomFoldoutHeader(string text, bool isExpanded, GUIStyle style)
        {
            string arrowDirectionSymbol = isExpanded ? "▾" : "▸";
            string displayText = $"{arrowDirectionSymbol} {text}";

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(displayText, style, GUILayout.ExpandWidth(true)))
                {
                    isExpanded = !isExpanded;
                }
            }

            return isExpanded;
        }
    }
}