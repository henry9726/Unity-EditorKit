using System.Collections.Generic;
using UnityEngine;

namespace Henry.EditorKit
{
    public class RecordStore : ScriptableObject
    {
        const string RecordStoreVersion = "1.0.0";

        [SerializeField] string recordStoreVersion = RecordStoreVersion;
        [SerializeField] List<Record> records = new();

        public IReadOnlyList<Record> Records => records;

        RecordStore() { }

        public void SetRecords(IEnumerable<Record> records)
        {
            this.records.Clear();
            this.records.AddRange(records);
        }

        public static RecordStore LoadRecord()
        {
            var result = RecordIO.Load() as RecordStore;

            if (result == null)
            {
                result = CreateInstance<RecordStore>();
            }
            else if (result.recordStoreVersion != RecordStoreVersion)
            {
                LogPrinter.PrintWarning($"The record store version is not match, expected {RecordStoreVersion}, actual {result.recordStoreVersion}, will create a new record store");
                result = CreateInstance<RecordStore>();
            }

            result.hideFlags = HideFlags.DontSaveInEditor;
            return result;
        }

        public void SaveRecord()
        {
            RecordIO.Save(this);
        }
    }
}