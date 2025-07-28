using UnityEditor;

namespace Tools
{
    public static class QuickSettings
    {

        #region Enter Play Mode Setting
        [MenuItem("Tools/Settings/Enter Play Mode Setting/Reload Domain and Scene", priority = 0)]
        public static void ReloadDomainandScene()
        {
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.None;
        }

        [MenuItem("Tools/Settings/Enter Play Mode Setting/Reload Scene only", priority = 1)]
        public static void ReloadSceneOnly()
        {
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
        }

        [MenuItem("Tools/Settings/Enter Play Mode Setting/Reload Domain only", priority = 2)]
        public static void ReloadDomainOnly()
        {
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableSceneReload;
        }

        [MenuItem("Tools/Settings/Enter Play Mode Setting/Do not reload Domain or Scene", priority = 3)]
        public static void DoNotReloadDomainOrScene()
        {
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;
        }
        #endregion


    }
}
