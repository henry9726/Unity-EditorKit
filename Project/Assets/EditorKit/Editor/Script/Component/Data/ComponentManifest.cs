using UnityEditor;
using UnityEngine;

namespace Henry.EditorKit.Component
{
    public class ComponentManifest : ScriptableObject
    {
        [SerializeField] MonoScript script;
        [SerializeField] Config config;

        public MonoScript Script => script;
        public Config Config => config;

        ComponentManifest()
        {
            config = new Config("");
        }

        [MenuItem("Assets/Create/EditorKit/Create Empty ComponentManifest", false, 0)]
        static void CreateComponentManifest()
        {
            var manifest = ScriptableObject.CreateInstance<ComponentManifest>();
            ProjectWindowUtil.CreateAsset(manifest, "New ComponentManifest.asset");
        }

        [MenuItem("Assets/Create/EditorKit/Generate ComponentManifest", false, 1)]
        static void GenerateComponentManifest()
        {
            var selectedScript = Selection.activeObject as MonoScript;
            if (selectedScript == null)
            {
                EditorUtility.DisplayDialog("Error", $"Please select a script.", "OK");
                return;
            }

            var targetType = selectedScript.GetClass();
            var icomponentName = typeof(IComponent).FullName;
            if (typeof(IComponent).IsAssignableFrom(targetType) is false)
            {
                EditorUtility.DisplayDialog("Error", $"Selected script is unvalid.\n\nPlease implement [{icomponentName}] interface.", "OK");
                return;
            }

            string scriptPath = AssetDatabase.GetAssetPath(selectedScript);
            string dir = System.IO.Path.GetDirectoryName(scriptPath);
            string manifestName = selectedScript.name + "Manifest.asset";
            string manifestPath = System.IO.Path.Combine(dir, manifestName);
            manifestPath = AssetDatabase.GenerateUniqueAssetPath(manifestPath);

            var manifest = ScriptableObject.CreateInstance<ComponentManifest>();
            manifest.script = selectedScript;
            manifest.config = new Config(selectedScript.name)
            {
                Author = System.Environment.UserName,
                Version = "1.0.0"
            };

            AssetDatabase.CreateAsset(manifest, manifestPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = manifest;
        }
    }
}
