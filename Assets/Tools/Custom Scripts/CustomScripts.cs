using UnityEditor;
using System.IO;
using UnityEngine;
using UnityEditor.ProjectWindowCallback;
using System;

namespace Tools
{

    public static class CustomScripts
    {
        [MenuItem("Tools/Custom Scripts/Create Script Template", priority = 50)]
        public static void CreateScriptTemplate()
        {
            //ScriptTemplate asset = ScriptableObject.CreateInstance<ScriptTemplate>();
            string root = "Assets";
            string directory = "Script Templates";
            string defaultAssetName = "NewScriptTemplate.asset";

            string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(root, "Tools", "Custom Scripts", directory, defaultAssetName));

            CreateAsset action = ScriptableObject.CreateInstance<CreateAsset>();
            action.Init(typeof(ScriptTemplate));

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            action,
            path,
            EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D,
            null
            );
        }

        private static string templatePath = Path.Combine("Assets", "Tools", "Custom Scripts", "Script Templates");

        private static void CreateScript(string templatePath)
        {
            if (!File.Exists(templatePath))
            {
                Debug.LogError($"The template at {templatePath} doesn't exist.");
                return;
            }

            ScriptTemplate template = AssetDatabase.LoadAssetAtPath<ScriptTemplate>(templatePath);

            if (template == null)
            {
                Debug.LogError("Script template not found at path: " + templatePath);
                return;
            }

            var action = ScriptableObject.CreateInstance<CreateScriptFromTemplateAction>();
            action.Init(template);

            string defaultName = template.DefaultName;
            if (!defaultName.EndsWith(".cs"))
                defaultName += ".cs";

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                action,
                Path.Combine(GetSelectedPathOrFallback(), defaultName),
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                null
            );   
        }

        #region MonoBehaviour

        private static string monoBehaviourTemplatePath = Path.Combine(templatePath, "MonoBehaviourTemplate.asset");
        [MenuItem("Assets/Create/Scripts/MonoBehaviour", false, 0)]
        public static void CreateMonoBehaviourScript()
        {
            CreateScript(monoBehaviourTemplatePath);
        }

        #endregion

        #region Manager

        private static string managerTemplatePath = Path.Combine(templatePath, "ManagerTemplate.asset");
        [MenuItem("Assets/Create/Scripts/Manager", false, 1)]
        public static void CreateManagerScript()
        {
            CreateScript(managerTemplatePath);
        }

        #endregion

        #region Scriptable Object

        private static string scriptableObjectTemplatePath = Path.Combine(templatePath, "ScriptableObjectTemplate.asset");
        [MenuItem("Assets/Create/Scripts/Scriptable Object", false, 3)]
        public static void CreateScriptableObjectScript()
        {
            CreateScript(scriptableObjectTemplatePath);
        }

        #endregion

        #region Class
        private static string classTemplatePath = Path.Combine(templatePath, "ClassTemplate.asset");
        [MenuItem("Assets/Create/Scripts/Class", false, 4)]
        public static void CreateClassScript()
        {
            CreateScript(classTemplatePath);
        }

        #endregion


        private static string GetSelectedPathOrFallback()
        {
            string root = "Assets";
            string path = root;
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path)) { path = Path.GetDirectoryName(path); }
                break;
            }
            return path;
        }

    }

    class CreateAsset : EndNameEditAction
    {
        private Type type;

        public void Init(Type type)
        {
            if (type == null)
            {
                Debug.LogError("Type was null.");
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

    public class CreateScriptFromTemplateAction : EndNameEditAction
    {
        private ScriptTemplate scriptTemplate;

        public void Init(ScriptTemplate template)
        {
            this.scriptTemplate = template;
        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string className = Path.GetFileNameWithoutExtension(pathName);
            string code = scriptTemplate.Template.Replace("ClassName", className);

            File.WriteAllText(pathName, code);
            AssetDatabase.Refresh();

            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}