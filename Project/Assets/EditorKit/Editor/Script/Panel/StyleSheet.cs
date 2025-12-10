using UnityEngine;
using UnityEditor;

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

        StyleSheet() { }

        public void Setup()
        {
            if (IsSetup) return;

            ColorUtility.TryParseHtmlString("#a5bfde", out Color h1TextColor);
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
            IsSetup = true;
        }
    }
}