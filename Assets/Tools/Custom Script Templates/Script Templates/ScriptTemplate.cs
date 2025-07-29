using UnityEngine;

namespace Tools
{
    public class ScriptTemplate : ScriptableObject
    {
        [Tooltip("The default name assigned to the script on creation.")]
        public string DefaultName;
        [Tooltip("The code template you want to load into a newly created script. Use placeholders <{placeholder}> for the script name (also class name).")]
        [TextArea(40, 40)]
        public string Template;
    }    
}

