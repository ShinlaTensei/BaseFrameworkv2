using System;
using System.Collections;
using System.Collections.Generic;
using Base.Logging;
using Base.Module;
using Base.Pattern;
using Base.Data.Structure;
using FileHelpers;
using UnityEngine;

namespace Base.Services
{
    [DelimitedRecord(",")]
    [IgnoreFirst]
    public record LocalizeDataRecord
    {
        public string Key { get; set; }
        public string VI  { get; set; }
        public string EN  { get; set; }
    }
    [BlueprintReader("Data/BlueprintLocalization", DataFormat.Csv, RemotePath = "BlueprintLocalization")]
    public class BlueprintLocalization : BaseBlueprintCsv<LocalizeDataRecord>
    {
        private Dictionary<string, string> m_localizeData = new Dictionary<string, string>();

        public override void Load()
        {
            LocalizeManager localizeManager = ServiceLocator.GetService<LocalizeManager>()!;
            if (Data != null && Data.Count > 0)
            {
                m_localizeData.Clear();
                foreach (var dataItem in Data)
                {
                    if (localizeManager.CurrentLanguage is LanguageCode.Vi)
                    {
                        m_localizeData.TryAdd(dataItem.Key, dataItem.VI);
                    }
                    else if (localizeManager.CurrentLanguage is LanguageCode.En)
                    {
                        m_localizeData.TryAdd(dataItem.Key, dataItem.EN);
                    }
                }
            }
        }

        public override void LoadDummyData()
        {
            throw new NotImplementedException();
        }

        public string GetTextByKey(string key)
        {
            if (m_localizeData.ContainsKey(key))
            {
                return m_localizeData[key];
            }
            PDebug.WarnFormat("[BlueprintLocalize] Missing localize text of ID ({0})", key);
            return string.Empty;
        }
    }
}

