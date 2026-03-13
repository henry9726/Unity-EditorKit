namespace Henry.EditorKit
{
    public class Priority
    {
        public const int MainPanel = 21;
    }

    public class MenuPath
    {
        public const string Root = "Tools";
        public const string EditorKitDisplayName = "✦ EditorKit";
        public const string EditorKitMenuPath = Root + "/" + EditorKitDisplayName;
    }

    public class RootConfig
    {
        public const string RepositoryUrl = "https://github.com/henry9726/Unity-EditorKit";

        static RootConfig()
        {
            var assembly = typeof(RootConfig).Assembly;
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assembly);

            if (packageInfo != null)
            {
                AssetsPath = $"Packages/{packageInfo.name}";
            }
            else
            {
                AssetsPath = "Assets/EditorKit";
            }
        }

        public static readonly string AssetsPath;
        public const string UserSettingsDirectory = "UserSettings";
        public const string EditorKitDirectory = "EditorKit";
        public const string ComponentRecordSystemVersion = "1.0.0";
    }
}
