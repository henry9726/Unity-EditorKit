using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Henry.EditorKit.BuiltInComponent
{
    class AppFpsSwitcher : ScriptableObject, IComponent
    {
        record Option(string Title, int Value);

        readonly string currentValueLabelPrefix = "Current";
        readonly string applyBtnText = "Apply";
        readonly string infiniteFpsText = "∞";

        GUILayoutOption[] toggleLayoutOptions;
        GUILayoutOption applyBtnMaxWidth;
        GUILayoutOption manualValueInputerMaxWidth;

        GUIStyle labelStyle;

        string currentValueText;
        int previousSetTimeScale = 1;
        int latestSetFps;
        int customFps;

        readonly Option[] defaultOptions = new Option[] {
            new ( "0", 0 ),
            new ( "30", 30 ),
            new ( "60", 60 ),
            new ( "120", 120 ),
            new ( "240", 240 ),
        };
        string[] optionsTitle;

        void IComponent.OnEnable()
        {
            optionsTitle = defaultOptions.Select(el => el.Title).ToArray();
            SetFps(Application.targetFrameRate);
        }

        void IComponent.OnGUI(Rect rect)
        {
            ValidateStyles();

            using (new EditorGUILayout.VerticalScope())
            {
                var isUseFps = Application.targetFrameRate;
                if (isUseFps != latestSetFps)
                {
                    SetFps(isUseFps);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(currentValueText))
                        SetFps(previousSetTimeScale);

                    customFps = EditorGUILayout.IntField(customFps, manualValueInputerMaxWidth);

                    if (GUILayout.Button(applyBtnText, applyBtnMaxWidth))
                        SetFps(customFps);
                }

                var selectedOptionIdx = GUILayout.Toolbar(GetSelectedIdxByValue(latestSetFps), optionsTitle, toggleLayoutOptions);
                if (selectedOptionIdx != -1 && defaultOptions[selectedOptionIdx].Value != latestSetFps)
                {
                    SetFps(defaultOptions[selectedOptionIdx].Value);
                }
            }
        }

        void ValidateStyles()
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.label);
            }
            if (applyBtnMaxWidth == null)
            {
                applyBtnMaxWidth = GUILayout.MaxWidth(labelStyle.CalcSize(new GUIContent($"__{applyBtnText}__")).x);
            }
            if (manualValueInputerMaxWidth == null)
            {
                manualValueInputerMaxWidth = GUILayout.MaxWidth(labelStyle.CalcSize(new GUIContent("__100_000__")).x);
            }
            if (toggleLayoutOptions == null)
            {
                toggleLayoutOptions = new GUILayoutOption[] { GUILayout.Height(22) };
            }
        }

        int GetSelectedIdxByValue(int value)
        {
            for (int i = 0; i < defaultOptions.Length; i++)
            {
                if (defaultOptions[i].Value == value)
                    return i;
            }

            if (IsInfiniteFps(value))
                return 0;

            return -1;
        }

        void SetFps(int value)
        {
            value = Math.Min(value, 100_000);

            previousSetTimeScale = latestSetFps;
            Application.targetFrameRate = latestSetFps = value;

            currentValueText = $"{currentValueLabelPrefix} : {(IsInfiniteFps(value) ? infiniteFpsText : value)} (⇆ {(IsInfiniteFps(previousSetTimeScale) ? infiniteFpsText : previousSetTimeScale)})";
        }

        bool IsInfiniteFps(int value) => value <= 0;
    }
}