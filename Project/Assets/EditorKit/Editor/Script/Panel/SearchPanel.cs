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
        IEnumerable<Info> searchResult = Array.Empty<Info>();
        Vector2 scrollPosition = Vector2.zero;

        public event Action<Info> OnRequestPinComp;
        public event Action<Info> OnRequestOpenComp;

        public SearchPanel()
        {
            componentInfoList = ComponentRegistry.List;
            searcher = SearcherManager.GetSearcher();
            searchResult = componentInfoList;
        }

        void ISubPanel.OnEnable()
        {
            style = StyleSheet.Instance;

            infoIcon = EditorGUIUtility.Load(RootConfig.AssetsPath + "/Editor/Icons/info_16dp.png") as Texture2D;
            openIcon = EditorGUIUtility.Load(RootConfig.AssetsPath + "/Editor/Icons/open_in_new_16dp.png") as Texture2D;
            keepIcon = EditorGUIUtility.Load(RootConfig.AssetsPath + "/Editor/Icons/keep_16dp.png") as Texture2D;

            infoIconContent = new GUIContent(infoIcon, "Open help");
            openIconContent = new GUIContent(openIcon, "Open in new window");
            keepIconContent = new GUIContent(keepIcon, "Pin it");

            componentInfoList = ComponentRegistry.List;
            searcher = SearcherManager.GetSearcher();
        }

        void ISubPanel.OnGUI(Rect rect)
        {
            ValidateStyles();
            DrawSearchBar();

            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = view.scrollPosition;
                foreach (var el in searchResult)
                {
                    DrawComponentCard(el);
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

        void DrawComponentCard(Info info)
        {
            var config = info.Config;
            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                EditorGUILayout.LabelField(config.Name, style.H1, style.Title_H1);

                if (!string.IsNullOrEmpty(config.Description))
                {
                    EditorGUILayout.LabelField(config.Description, EditorStyles.wordWrappedLabel);
                }

                Color originalColor = GUI.contentColor;
                GUI.contentColor = gray75;
                EditorGUILayout.LabelField($"Author  : {config.Author}", GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField($"Version : {config.Version}", GUILayout.ExpandWidth(false));
                GUI.contentColor = originalColor;

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
    }
}