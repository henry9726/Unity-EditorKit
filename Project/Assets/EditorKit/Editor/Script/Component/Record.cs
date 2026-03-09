using System;

namespace Henry.EditorKit
{
    [Serializable]
    public class Record
    {
        public string compTypeFullName;
        public string compContent;
        public bool isExpanded;

        public Record(string compTypeFullName)
        {
            this.compTypeFullName = compTypeFullName;
            isExpanded = true;
        }

        public void SetExpanded(bool expanded)
        {
            isExpanded = expanded;
        }
    }
}