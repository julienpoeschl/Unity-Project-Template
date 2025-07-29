using System;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


public static class PackageInstaller
{
    static AddRequest request;

    private static string packageListDir = Path.Combine("Assets", "Tools", "Setup", "Package Installer", "Package Lists");

    public static void Install()
    {

        // Search for obj
        // Find all entries
        // Install all entries
        string packageListAssetName = "PackageList.asset";

        PackageListSO packageList = AssetDatabase.LoadAssetAtPath<PackageListSO>(Path.Combine(packageListDir, packageListAssetName));

        for (int i = 0; i < packageList.packages.Length; i++)
        {
            string package = packageList.packages[i];
            InstallPackage(package);
        }

    }

    private static void InstallPackage(string package)
    {
        try
        {
            request = Client.Add(package);
            EditorApplication.update += Progress;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Package {package} couldn't be installed. Make sure the link is up to date.");
        }
    }

    static void Progress()
    {
        if (request.IsCompleted)
        {
            if (request.Status == StatusCode.Success)
                Debug.Log("Installed: " + request.Result.packageId);
            else if (request.Status >= StatusCode.Failure)
                Debug.LogError("Failed to install package: " + request.Error.message);

            EditorApplication.update -= Progress;
        }
    }
}
