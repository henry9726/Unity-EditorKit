using System;
using UnityEngine;

namespace Henry.EditorKit.Component
{
    [Serializable]
    public class Config
    {
        [field: SerializeField] public string Name { get; set; } = string.Empty;
        [field: SerializeField, TextArea] public string Description { get; set; } = string.Empty;
        [field: SerializeField] public string Author { get; set; } = string.Empty;
        [field: SerializeField] public string Version { get; set; } = string.Empty;
        [field: SerializeField] public string ReadmePath { get; set; } = string.Empty;

        public Config(string name)
        {
            Name = name;
        }
    }
}