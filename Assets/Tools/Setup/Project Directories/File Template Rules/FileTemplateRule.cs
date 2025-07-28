using UnityEngine;

namespace Tools
{
    public class FileTemplateRule : ScriptableObject
    {
        public string RegexRule;
        public uint Priority;
        [TextArea()]
        public string Template;
    }    
}
