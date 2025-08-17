using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PackageListSO : ScriptableObject
{
    [SerializeField] private Package[] packages;
    public Package[] Packages => packages;

}

[System.Serializable]
public class Package
{
    [SerializeField] private string name;
    [SerializeField] private string version;
    public string Name => name;
    public string Version => version;
}

public class PackageChecker
{
    private static ListRequest listRequest;
    private static SearchRequest searchRequest;

    // Check if installed
    public static void CheckIfInstalled(string packageName)
    {
        listRequest = Client.List(); // List installed packages
        EditorApplication.update += CheckInstalledProgress;

        void CheckInstalledProgress()
        {
            if (listRequest.IsCompleted)
            {
                foreach (var p in listRequest.Result)
                {
                    if (p.name == packageName)
                        Debug.Log($"✅ {packageName} is installed (version {p.version})");
                }
                EditorApplication.update -= CheckInstalledProgress;
            }
        }
    }

    // Check if exists on registry
    public static void CheckIfAvailable(string packageName)
    {
        searchRequest = Client.Search(packageName);
        EditorApplication.update += CheckAvailableProgress;

        void CheckAvailableProgress()
        {
            if (searchRequest.IsCompleted)
            {
                foreach (var p in searchRequest.Result)
                {
                    Debug.Log($"Found {p.name}, version {p.version}");
                }
                EditorApplication.update -= CheckAvailableProgress;
            }
        }
    }
}