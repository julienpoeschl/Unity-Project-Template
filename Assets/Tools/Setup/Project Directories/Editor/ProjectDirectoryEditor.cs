using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Tools
{
    
    [CustomEditor(typeof(ProjectDirectory))]
    public class ProjectDirectoryEditor : Editor
    {
        private ProjectDirectory projectDirectory;
        private Dictionary<Directory, bool> directoryFoldouts = new Dictionary<Directory, bool>();
        private Color baseColor;


        private void OnEnable()
        {
            projectDirectory = (ProjectDirectory)target;
            baseColor = GUI.backgroundColor;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Project Directory Root", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Root action buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Directory"))
            {
                AddSubdirectory(ref projectDirectory.Directories);
            }

            if (GUILayout.Button("Add File"))
            {
                AddFile(ref projectDirectory.Directories);
            }

            if (GUILayout.Button("Validate entire Project Directory"))
            {
                projectDirectory.IsValidProjectDirectory();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Render root subdirectories
            if (projectDirectory.Directories == null) return;

            for (int i = 0; i < projectDirectory.Directories.Length; i++)
            {
                RenderDirectory(projectDirectory.Directories[i], ref projectDirectory.Directories, i, 0);
                GUILayout.Space(10);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderDirectory(Directory directory, ref Directory[] parentArray, int indexInParent, int depth)
        {
            // Box container
            EditorGUILayout.BeginVertical("box");

            ///
            //  float intensity = Mathf.Clamp01(depth * 0.0001f); // darkens faster
            //  Color newColor = new Color(baseColor.r * intensity, baseColor.g * intensity, baseColor.b * intensity);
            //  GUI.backgroundColor = newColor;
            /// 

            // Header line: Foldout on the left, "X" remove button on the right
            if (!directoryFoldouts.ContainsKey(directory))
                directoryFoldouts[directory] = true;

            EditorGUILayout.BeginHorizontal();

            // Foldout (toggle with directory name)
            directoryFoldouts[directory] = EditorGUILayout.Foldout(directoryFoldouts[directory], directory.Name, true);

            // Buttons: Validate, Add Subd., Remove
            if (GUILayout.Button("Validate", GUILayout.Width(70)))
            {
                directory.ContainsConflictingNames(); // If you have a custom method on Directory, otherwise call on projectDirectory
            }

            if (GUILayout.Button("Add Subd.", GUILayout.Width(80)))
            {
                AddSubdirectory(ref directory.Subdirectories);
            }

            // Remove button aligned to the top right
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                RemoveAt(ref parentArray, indexInParent);
                EditorGUILayout.EndHorizontal(); // Close header
                EditorGUILayout.EndVertical();   // Close box
                return; // Don't draw contents of a deleted directory
            }
            EditorGUILayout.EndHorizontal();

            // If collapsed, don't draw contents
            if (!directoryFoldouts[directory])
            {
                EditorGUILayout.EndVertical();
                return;
            }


            directory.Name = EditorGUILayout.TextField("Name", directory.Name);

            // Files
            EditorGUILayout.BeginHorizontal();

            // Label on the far left
            EditorGUILayout.LabelField("FileNames", GUILayout.Width(70));

            // Flexible space in between pushes next content to the right
            GUILayout.FlexibleSpace();

            // Vertical group on the right side
            EditorGUILayout.BeginVertical(GUILayout.Width(180)); // 150 + 20 (button)
            for (int i = 0; i < directory.FileNames.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                directory.FileNames[i] = EditorGUILayout.TextField(directory.FileNames[i], GUILayout.Width(150));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    RemoveAt(ref directory.FileNames, i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            // Add File button
            if (GUILayout.Button("Add File", GUILayout.Width(170)))
            {
                AddFile(ref directory.FileNames);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Subdirectories");

            if (directory.Subdirectories != null)
            {
                for (int i = 0; i < directory.Subdirectories.Length; i++)
                {
                    if (i > 0) GUILayout.Space(5);
                    RenderDirectory(directory.Subdirectories[i], ref directory.Subdirectories, i, depth + 1);
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void AddSubdirectory(ref Directory[] dirs)
        {
            if (dirs == null)
                dirs = new Directory[0];

            var newDirs = new Directory[dirs.Length + 1];
            dirs.CopyTo(newDirs, 0);
            newDirs[dirs.Length] = new Directory { Name = "NewDir", Subdirectories = new Directory[0], FileNames = new string[0] };
            dirs = newDirs;
            EditorUtility.SetDirty(projectDirectory);
        }

        private void AddFile(ref string[] files)
        {
            if (files == null)
                files = new string[0];

            var newFiles = new string[files.Length + 1];
            files.CopyTo(newFiles, 0);
            newFiles[files.Length] = "NewFile";
            files = newFiles;
            EditorUtility.SetDirty(projectDirectory);
        }

        private void AddFile(ref Directory[] dirs)
        {
            if (dirs.Length > 0)
            {
                AddFile(ref dirs[0].FileNames);
            }
        }

        private void RemoveAt<T>(ref T[] array, int index)
        {
            var newArray = new T[array.Length - 1];
            for (int i = 0, j = 0; i < array.Length; i++)
            {
                if (i == index) continue;
                newArray[j++] = array[i];
            }
            array = newArray;
            EditorUtility.SetDirty(projectDirectory);
        }
    }
}
