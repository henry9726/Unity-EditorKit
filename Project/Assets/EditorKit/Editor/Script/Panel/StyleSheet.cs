using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Henry.EditorKit
{
    public class StyleSheet
    {
        public static StyleSheet Instance { get; } = new();

        public bool IsSetup { get; private set; }

        public GUIStyle H1 { get; private set; }
        public GUILayoutOption[] Title_H1 { get; private set; }
        public GUILayoutOption[] FieldLabel_md { get; private set; }
        public GUIStyle Block { get; private set; }
        public GUIStyle Button { get; private set; }
        public GUILayoutOption Button_md { get; private set; }
        public GUILayoutOption Button_lg { get; private set; }
        public GUILayoutOption Button_xl { get; private set; }
        public GUILayoutOption Button_min_md { get; private set; }

        public GUIStyle ExpandedFoldoutHeaderStyle { get; private set; }
        public GUIStyle CollapsedFoldoutHeaderStyle { get; private set; }

        Color H1TextColor { get; set; }
        Color DisabledTextColor { get; set; }

        StyleSheet() { }

        public void Setup()
        {
            if (IsSetup) return;

            ColorUtility.TryParseHtmlString("#a5bfde", out Color h1TextColor);
            H1TextColor = h1TextColor;
            H1 = new(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold, alignment = TextAnchor.UpperLeft };
            H1.normal.textColor = h1TextColor;

            Title_H1 = new[] { GUILayout.Height(20) };
            FieldLabel_md = new[] { GUILayout.Width(100) };
            Block = new(EditorStyles.helpBox) { padding = new RectOffset(5, 5, 5, 5) };
            Button = new(GUI.skin.button) { fontSize = 12 };
            Button_md = GUILayout.Width(100);
            Button_lg = GUILayout.Width(200);
            Button_xl = GUILayout.Width(300);
            Button_min_md = GUILayout.MinWidth(100);

            ExpandedFoldoutHeaderStyle = CreateFoldoutStyle(H1, H1TextColor);

            ColorUtility.TryParseHtmlString("#989898", out Color disabledTextColor);
            DisabledTextColor = disabledTextColor;
            CollapsedFoldoutHeaderStyle = CreateFoldoutStyle(H1, DisabledTextColor);

            IsSetup = true;
        }

        GUIStyle CreateFoldoutStyle(GUIStyle textStyle, Color textColor)
        {
            var stateStyle = new GUIStyleState() { textColor = textColor };
            var style = new GUIStyle(textStyle)
            {
                normal = stateStyle,
                hover = stateStyle,
                active = stateStyle
            };
            return style;
        }

        public void ApplyExpandedFoldoutHeaderStyle(VisualElement element)
        {
            ApplyBaseFoldoutStyle(element, H1TextColor);
        }

        public void ApplyCollapsedFoldoutHeaderStyle(VisualElement element)
        {
            ApplyBaseFoldoutStyle(element, DisabledTextColor);
        }

        void ApplyBaseFoldoutStyle(VisualElement element, Color color)
        {
            element.style.fontSize = 15;
            element.style.unityFontStyleAndWeight = FontStyle.Bold;
            element.style.color = color;
        }
    }
}