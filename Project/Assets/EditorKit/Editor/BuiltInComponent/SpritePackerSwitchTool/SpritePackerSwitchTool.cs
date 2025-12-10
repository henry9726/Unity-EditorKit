using UnityEditor;
using UnityEngine;

namespace Henry.EditorKit.BuiltInComponent
{
    using Henry.EditorKit.Component;

    class SpritePackerSwitchTool : ScriptableObject, IComponent
    {
        readonly string[] optionsTitle = new string[] { "Disable", "V1", "V2", "Other" };

        int usingStateIndex;

        public static Config Info => new("SpritePacker Switcher")
        {
            Author = "林祐豪、李育杰",
            Version = "1.0.1"
        };

        void IComponent.OnEnable()
        {
            usingStateIndex = GetCurrentPackerStateIndex();
        }

        void IComponent.OnDisable() { }

        void IComponent.OnGUI(Rect rect)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var tempSelectedStateIndex = DrawToolbar();

                if (tempSelectedStateIndex != 3 && tempSelectedStateIndex != usingStateIndex)
                {
                    if (ShowConfirmDialog(optionsTitle[tempSelectedStateIndex]))
                    {
                        usingStateIndex = tempSelectedStateIndex;
                        switch (usingStateIndex)
                        {
                            case 0: SetPackerMode(SpritePackerMode.Disabled); break;
                            case 1: SetPackerMode(SpritePackerMode.AlwaysOnAtlas); break;
                            case 2: SetPackerMode(SpritePackerMode.SpriteAtlasV2); break;
                        }
                    }
                }
            }

            usingStateIndex = GetCurrentPackerStateIndex();
        }

        int DrawToolbar()
        {
            var isDisabled = DrawToggle(optionsTitle[0], usingStateIndex == 0, "ButtonLeft", true);
            var isAlwaysOnAtlas = DrawToggle(optionsTitle[1], usingStateIndex == 1, "ButtonMid", true);
            var isSpriteAtlasV2 = DrawToggle(optionsTitle[2], usingStateIndex == 2, "ButtonMid", true);
            DrawToggle(optionsTitle[3], usingStateIndex == 3, "ButtonRight", false);

            return
            isDisabled ? 0
            : isAlwaysOnAtlas ? 1
            : isSpriteAtlasV2 ? 2
            : 3;
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
            _ => 3
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

        static bool DrawToggle(string text, bool isActive, string style, bool isEnable)
        {
            var result = false;
            var tempGuiEnabled = GUI.enabled;

            GUI.enabled = isEnable;
            if (GUILayout.Toggle(isActive, text, style) != isActive)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            GUI.enabled = tempGuiEnabled;
            return result;
        }
    }
}
