using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Setup
{
    private static bool settings = false;
    private static bool directories = false;

    private static string monoBehaviourScriptTemplate = string.Empty;

    [MenuItem("Tools/Setup/Create Project Setup")]
    public static void CreateProjectSetup()
    {
        Debug.Log("Set Editor Settings");

        Debug.Log("Create Directories");
        string projectDirectoryPath = Path.Combine("Assets", "Tools", "Setup", "ProjectDirectory.asset");
        ProjectDirectory projectDirectory = AssetDatabase.LoadAssetAtPath<ProjectDirectory>(projectDirectoryPath);
        if (!projectDirectory)
        {
            Debug.LogError($"The Project Directory Asset wasn't found at {projectDirectoryPath}. Make sure it exists.");
            return;
        }
        Debug.Log("The Project Directory was found. Checking if it is valid...");
        if (!projectDirectory.IsValidProjectDirectory()) return;
        CreateProjectDirectoryStructure(projectDirectory);

        AssetDatabase.Refresh();
        Debug.Log("Finished setup.");
    }

    private static void CreateProjectDirectoryStructure(ProjectDirectory projectDirectory)
    {
        string rootName = projectDirectory.Root.Name;

        for (int i = 0; i < projectDirectory.Directories.Length; i++)
        {
            CreateDirectoryStructure(projectDirectory.Directories[i], rootName);
        }
    }

    private static void CreateDirectoryStructure(Directory directory, string rootPath)
    {
        string directoryPath = Path.Combine(rootPath, directory.Name);

        if (!LoadMonobehaviourScriptTemplate(out monoBehaviourScriptTemplate)) return;

        if (System.IO.Directory.Exists(directoryPath))
        {
            Debug.LogWarning($"The directory {directoryPath} already exists. No subdirectories of this directory will be created.");
            return;
        }

        System.IO.Directory.CreateDirectory(directoryPath);

        if (directory.FileNames != null)
        {
            for (int i = 0; i < directory.FileNames.Length; i++)
            {
                string fileName = directory.FileNames[i];
                string filePath = Path.Combine(directoryPath, fileName);
                if (File.Exists(filePath)) continue;

                if (fileName.EndsWith(".cs"))
                {
                    string fileNameWithoutExtension = fileName.Replace(".cs", ""); // Error prone
                    if (fileNameWithoutExtension.Equals(string.Empty))
                    {
                        fileNameWithoutExtension = "Default";
                    }
                    string specificTemplate = string.Format(monoBehaviourScriptTemplate, fileNameWithoutExtension);
                    File.WriteAllText(filePath, specificTemplate);
                }
                else
                {
                    File.WriteAllText(filePath, string.Empty);
                }
            }
        }

        for (int i = 0; i < directory.Subdirectories.Length; i++)
        {
            Directory subdirectory = directory.Subdirectories[i];
            string newRootName = Path.Combine(rootPath, directory.Name);
            CreateDirectoryStructure(subdirectory, newRootName);
        }
    }

    private static bool LoadMonobehaviourScriptTemplate(out string template)
    {
        template = string.Empty;
        string path = Path.Combine("Assets", "Tools", "Setup", "MonoBehaviourScriptTemplate.txt");
        if (!File.Exists(path))
        {
            Debug.LogError($"The Template for MonoBehaviour Scripts doesn't exist. Please make sure that {path} exists./");
            return false;
        }
        template = File.ReadAllText(path);
        return true;

    }
}
