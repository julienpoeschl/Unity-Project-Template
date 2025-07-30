using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{

    public class ProjectDirectorySO : ScriptableObject
    {
        [Tooltip("The nested project directory structure.")]
        public Directory[] Directories;
        public Directory Root
        {
            get
            {
                Directory root = new Directory
                {
                    Name = "Assets",
                    Subdirectories = Directories,
                    FileNames = Array.Empty<string>()
                };
                return root;
            }
        }

        [ContextMenu("Check Project Directory valitidy")]
        public bool IsValidProjectDirectory()
        {

            return !Root.ContainsConflictingNames();
        }

    }

    [Serializable]
    public class Directory
    {
        public string Name;
        public Directory[] Subdirectories;
        public string[] FileNames;

        /// <summary>
        /// Finds the first duplicate directory or file name in the subdirectories of the given directory and warns the user with a log statement.
        /// </summary>
        /// <param name="directory">The directory to check for duplicate names in subdirectories.</param>
        /// <returns>True if duplicate name in same subdirectory was found, else false.</returns>
        [ContextMenu("Check Directory valitidy")]
        public bool ContainsConflictingNames()
        {
            if (Subdirectories.Length == 0 && FileNames.Length == 0) return false;

            HashSet<string> uniqueFileNames = new HashSet<string>();

            for (int j = 0; j < FileNames.Length; j++)
            {
                string fileName = FileNames[j];
                if (uniqueFileNames.Contains(fileName))
                {
                    Debug.LogError($"The Directory has duplicate File Names in the same Subdirectory \"{fileName}\" in \"{Name}\".");
                    return true;
                }
                uniqueFileNames.Add(fileName);
            }

            HashSet<string> uniqueSubdirectoryNames = new HashSet<string>();

            for (int i = 0; i < Subdirectories.Length; i++)
            {
                Directory subdirectory = Subdirectories[i];

                if (uniqueSubdirectoryNames.Contains(subdirectory.Name))
                {
                    Debug.LogError($"The Directory has duplicate Directory Names in the same Subdirectory \"{subdirectory.Name}\" in \"{Name}\".");
                    return true;
                }
                uniqueSubdirectoryNames.Add(subdirectory.Name);

                if (subdirectory.ContainsConflictingNames()) return true;
            }

            Debug.Log($"The Directory is valid.");
            return false;
        }
    }
}
