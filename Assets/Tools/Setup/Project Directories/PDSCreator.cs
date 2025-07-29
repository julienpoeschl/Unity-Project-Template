using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public static class PDSCreator
    {
        private static string pdDir = Path.Combine("Assets", "Tools", "Setup", "Project Directories");
        private static string pdAssetPath = Path.Combine(pdDir, "ProjectDirectory.asset");
        private static string ftrDir = Path.Combine(pdDir, "File Template Rules");
        private static FileTemplateRule[] fileTemplateRules;

        public static bool Create()
        {
            ProjectDirectory projectDirectory = AssetDatabase.LoadAssetAtPath<ProjectDirectory>(pdAssetPath);
            if (!projectDirectory)
            {
                Debug.LogError($"The Project Directory Asset wasn't found at {pdAssetPath}. Make sure it exists.");
                return false;
            }
            Debug.Log("The Project Directory was found. Checking if it is valid...");
            if (!projectDirectory.IsValidProjectDirectory()) return false;
            fileTemplateRules = FindAll<FileTemplateRule>();
            Debug.Log(fileTemplateRules);
            CreateProjectDirectoryStructure(projectDirectory);
            AssetDatabase.Refresh();
            return true;
        }

        public static T[] FindAll<T>() where T : ScriptableObject
        {
            List<T> foundAssets = new List<T>();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            foreach (string guid in guids)
            {
                // Convert GUID to asset path
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // Load the asset as T
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset != null)
                {
                    foundAssets.Add(asset);
                }
            }

            return foundAssets.ToArray();
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
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                        if (fileNameWithoutExtension.Equals(string.Empty))
                        {
                            Debug.LogWarning("No file name. Was not created");
                            continue;
                        }

                        if (FindMatchingFileTemplateRule(fileName, out string code))
                        {
                            try
                            {
                                code = string.Format(code, fileNameWithoutExtension);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                            }
                            
                        }

                        File.WriteAllText(filePath, code);
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

        private static bool FindMatchingFileTemplateRule(string fileName, out string template)
        {
            template = string.Empty;
            bool found = false;
            uint currentPriority = uint.MaxValue;

            for (int i = 0; i < fileTemplateRules.Length; i++)
            {
                FileTemplateRule ftr = fileTemplateRules[i];

                if (Regex.IsMatch(fileName, ftr.RegexRule) && currentPriority > ftr.Priority)
                {
                    template = ftr.Template;
                    currentPriority = ftr.Priority;
                    found = true;
                }
            }

            return found;
        }
    }
}
