using UnityEditor;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ProjectSettingsSO : ScriptableObject
{
    public CustomEnterPlayModeOptions EnterPlayModeOptions;
    public EditorSettings.NamingScheme NamingScheme;

}

public enum CustomEnterPlayModeOptions
{
    //
    // Summary:
    //     This makes Unity reload the .NET Application Domain and entire Scene when entering
    //     Play Mode.
    DisableNone = EnterPlayModeOptions.None,
    //
    // Summary:
    //     When Domain Reload is disabled, scripts are not reloaded when entering Play Mode.
    //     This makes it quicker to switch to Play Mode, because there's no need to destroy,
    //     create and reload the .NET Application Domain.
    DisableDomainReload = EnterPlayModeOptions.DisableDomainReload,
    //
    // Summary:
    //     When Scene Reload is disabled, Unity resets the Scene state and emulates all
    //     of the required post-processor calls when entering Play Mode, instead of reloading
    //     the whole Scene. This makes it quicker to switch to Play Mode, because there's
    //     no need to destroy, create and awaken all the Scene objects, and serialize and
    //     deserialize the Scene from disk.
    DisableSceneReload = EnterPlayModeOptions.DisableSceneReload,
    ///
    /// 
    /// 
    DisableBoth = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload
}
