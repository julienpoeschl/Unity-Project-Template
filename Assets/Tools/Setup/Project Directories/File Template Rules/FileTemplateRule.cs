using UnityEngine;

namespace Tools
{
    public class FileTemplateRule : ScriptableObject
    {
        [Tooltip("")]
        public string RegexRule;
        [Tooltip("")]
        public uint Priority;
        [Tooltip("")]
        [TextArea(40, 40)]
        public string Template;
    }    
}
