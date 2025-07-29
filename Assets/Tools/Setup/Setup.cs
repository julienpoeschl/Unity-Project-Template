using System;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace Tools
{
    public static class Setup
    {

        [MenuItem("Tools/Setup/Create Project Setup", priority = 0)]
        public static void CreateProjectSetup()
        {
            Debug.Log("Set Editor Settings..");
            if (!CustomProjectSettings.Initialize()) return;
            Debug.Log("Finished initializing settings.");

            Debug.Log("Create Directories...");
            if (!PDSCreator.Create()) return;
            Debug.Log("Finished creating project structure.");

            Debug.Log("Installing packages..");
            if (!PackageInstaller.Install()) return;
            Debug.Log("Finished installing packages.");

            Debug.Log("Finished setup.");
        }

        [MenuItem("Tools/Setup/Create Project Directory", priority = 50)]
        public static void CreateProjectDirectory()
        {
            string root = "Assets";
            string directory = "Project Directories";
            string defaultAssetName = "NewProjectDirectory.asset";

            string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(root, "Tools", "Setup", directory, defaultAssetName));

            CreateAsset action = ScriptableObject.CreateInstance<CreateAsset>();
            action.Init(typeof(ProjectDirectory));

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            action,
            path,
            EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D,
            null
            );
        }

        [MenuItem("Tools/Setup/Create File Template Rule", priority = 51)]
        public static void CreateFileTemplateRule()
        {
            string root = "Assets";
            string directory = "File Template Rules";
            string defaultAssetName = "NewScriptTemplateRule.asset";

            string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(root, "Tools", "Setup", "Project Directories", directory, defaultAssetName));

            CreateAsset action = ScriptableObject.CreateInstance<CreateAsset>();
            action.Init(typeof(FileTemplateRule));

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            action,
            path,
            EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D,
            null
            );
        }

        [MenuItem("Tools/Setup/Create Project Settings", priority = 52)]
        public static void CreateProjectSettings()
        {
            string root = "Assets";
            string directory = "Project Settings";
            string defaultAssetName = "ProjectSettings.asset";

            string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(root, "Tools", "Setup", directory, defaultAssetName));

            CreateAsset action = ScriptableObject.CreateInstance<CreateAsset>();
            action.Init(typeof(ProjectSettingsSO));

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            action,
            path,
            EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D,
            null
            );
        }

        [MenuItem("Tools/Setup/Create Package List", priority = 53)]
        public static void CreatePackageList()
        {
            string root = "Assets";
            string directory = "Package Lists";
            string defaultAssetName = "PackageList.asset";

            string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(root, "Tools", "Setup", "Package Installer", directory, defaultAssetName));

            CreateAsset action = ScriptableObject.CreateInstance<CreateAsset>();
            action.Init(typeof(PackageListSO));

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            action,
            path,
            EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D,
            null
            );
        }

        class CreateAsset : EndNameEditAction
        {
            private Type type;

            public void Init(Type type)
            {
                if (type == null || !typeof(ScriptableObject).IsAssignableFrom(type))
                {
                    Debug.LogError("Invalid type passed to CreateAsset.");
                    return;
                }

                this.type = type;
            }

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                ScriptableObject asset = CreateInstance(type);
                AssetDatabase.CreateAsset(asset, pathName);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }
        }
    }
}

