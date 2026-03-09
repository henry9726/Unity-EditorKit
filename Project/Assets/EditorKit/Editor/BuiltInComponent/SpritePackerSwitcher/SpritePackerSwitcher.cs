using UnityEditor;
using UnityEngine;

namespace Henry.EditorKit.BuiltInComponent
{
    class SpritePackerSwitcher : ScriptableObject, IComponent
    {
        readonly string[] optionsTitle = new string[] { "Disable", "V1", "V2", "V1Build", "V2Build" };

        int usingStateIndex;

        void IComponent.OnEnable()
        {
            usingStateIndex = GetCurrentPackerStateIndex();
        }

        void IComponent.OnDisable() { }

        void IComponent.OnGUI(Rect rect)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var tempActiveIndex = DrawToolbar();

                if (tempActiveIndex != usingStateIndex)
                {
                    if (ShowConfirmDialog(optionsTitle[tempActiveIndex]))
                    {
                        usingStateIndex = tempActiveIndex;
                        switch (usingStateIndex)
                        {
                            case 0: SetPackerMode(SpritePackerMode.Disabled); break;
                            case 1: SetPackerMode(SpritePackerMode.AlwaysOnAtlas); break;
                            case 2: SetPackerMode(SpritePackerMode.SpriteAtlasV2); break;
                            case 3: SetPackerMode(SpritePackerMode.BuildTimeOnlyAtlas); break;
                            case 4: SetPackerMode(SpritePackerMode.SpriteAtlasV2Build); break;
                        }
                    }
                }
            }

            usingStateIndex = GetCurrentPackerStateIndex();
        }

        int DrawToolbar()
        {
            var rect = EditorGUILayout.GetControlRect();
            var count = optionsTitle.Length;
            var width = rect.width / count;
            var newIndex = usingStateIndex;

            for (int i = 0; i < count; i++)
            {
                var style = i == 0 ? "ButtonLeft" : i == count - 1 ? "ButtonRight" : "ButtonMid";
                var buttonRect = new Rect(rect.x + i * width, rect.y, width, rect.height);

                if (GUI.Toggle(buttonRect, usingStateIndex == i, optionsTitle[i], style))
                {
                    if (usingStateIndex != i)
                    {
                        newIndex = i;
                    }
                }
            }

            return newIndex;
        }

        static void SetPackerMode(SpritePackerMode mode)
        {
            EditorSettings.spritePackerMode = mode;
            AssetDatabase.SaveAssets();
        }

        static int GetCurrentPackerStateIndex() => EditorSettings.spritePackerMode switch
        {
            SpritePackerMode.Disabled => 0,
            SpritePackerMode.AlwaysOnAtlas => 1,
            SpritePackerMode.SpriteAtlasV2 => 2,
            SpritePackerMode.BuildTimeOnlyAtlas => 3,
            SpritePackerMode.SpriteAtlasV2Build => 4,
            _ => 0
        };

        static bool ShowConfirmDialog(string modeName)
        {
            return EditorUtility.DisplayDialog(
                "Confirm",
                $"You are about to change the SpritePacker mode to [{modeName}]. Are you sure?",
                "Confirm",
                "Cancel"
            );
        }
    }
}
