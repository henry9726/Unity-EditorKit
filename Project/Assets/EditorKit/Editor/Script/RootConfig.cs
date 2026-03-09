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

#if EDITORKIT_ENV_DEV
        // 1️⃣ Project Path
        public const string AssetsPath = "Assets/EditorKit";
#else
        // 2️⃣ Package Path
        public const string AssetsPath = "Packages/com.henrylin.editorkit";
#endif

        public const string UserSettingsDirectory = "UserSettings";
        public const string EditorKitDirectory = "EditorKit";

        public const string RecordSystemVersion = "1.0.0";
    }
}
