using System.Linq;
using Unity.CodeEditor;
using UnityEditor;
using UnityEngine;

namespace Henry.EditorKit.BuiltInComponent
{
    using Henry.EditorKit.Component;

    class CodeEditorTool : ScriptableObject, IComponent
    {
        readonly string selectorHeaderText = "Select";
        readonly string domainReloadBtnText = "Reload Domain";
        readonly string openProjectBtnText = "Open C# Project";
        readonly string openProjectCommandPath = "Assets/Open C# Project";

        GUIStyle labelStyle;
        GUILayoutOption selectorHeaderMaxWidth;

        CodeEditor codeEditor;
        string[] editorOptionsPath;
        string[] editorOptionsName;

        int selectedOptionIdx;

        public static Config Info => new("CodeEditor Tool")
        {
            Author = "林祐豪",
            Version = "1.0.1"
        };

        void IComponent.OnEnable()
        {
            codeEditor = CodeEditor.Editor;

            var editorOptionsNameDict = codeEditor.GetFoundScriptEditorPaths();
            editorOptionsPath = editorOptionsNameDict.Keys.ToArray();
            editorOptionsName = editorOptionsNameDict.Values.ToArray();

            FetchCurrentEditor();
        }

        void IComponent.OnGUI(Rect rect)
        {
            ValidateStyles();
            FetchCurrentEditor();

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(selectorHeaderText, selectorHeaderMaxWidth);
                    var tempSelectedIdx = EditorGUILayout.Popup(selectedOptionIdx, editorOptionsName);

                    if (tempSelectedIdx != selectedOptionIdx)
                    {
                        if (ShowConfirmDialog(editorOptionsName[tempSelectedIdx]))
                        {
                            selectedOptionIdx = tempSelectedIdx;
                            SetEditor(selectedOptionIdx);
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(openProjectBtnText))
                    {
                        CodeEditor.Editor.CurrentCodeEditor.SyncAll();
                        EditorApplication.ExecuteMenuItem(openProjectCommandPath);
                    }
                    if (GUILayout.Button(domainReloadBtnText))
                    {
                        EditorUtility.RequestScriptReload();
                    }
                }
            }
        }

        void FetchCurrentEditor()
        {
            var installationEditor = codeEditor.CurrentInstallation.Name;
            selectedOptionIdx = GetEditorIndexByName(installationEditor);
        }

        void ValidateStyles()
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.label);
            }
            if (selectorHeaderMaxWidth == null)
            {
                selectorHeaderMaxWidth = GUILayout.MaxWidth(labelStyle.CalcSize(new GUIContent(selectorHeaderText)).x);
            }
        }

        int GetEditorIndexByName(string editorName)
        {
            for (int i = 0; i < editorOptionsName.Length; i++)
            {
                if (editorOptionsName[i] == editorName)
                    return i;
            }
            return -1;
        }

        void SetEditor(int selectedOptionIdx)
        {
            if (selectedOptionIdx == -1 || selectedOptionIdx >= editorOptionsName.Length)
            {
                this.selectedOptionIdx = selectedOptionIdx;
            }
            else
            {
                var targetPath = editorOptionsPath[selectedOptionIdx];
                CodeEditor.SetExternalScriptEditor(targetPath);

                this.selectedOptionIdx = selectedOptionIdx;
            }
        }

        bool ShowConfirmDialog(string content)
        {
            return EditorUtility.DisplayDialog(
                "Confirm",
                $"You are about to change the Script Editor to \n> {content}\nAre you sure?",
                "Confirm",
                "Cancel"
            );
        }
    }
}