using System;
using System.Collections;
using System.Collections.Generic;
using Base.Logging;
using Base.Module;
using Base.Pattern;
using Base.Data.Structure;
using UnityEngine;

namespace Base.Services
{
    public class BlueprintLocalization : BaseBlueprint<LocalizeDataStructure>
    {
        private Dictionary<string, LocalizeDataItem> m_localizeData;

        public override void Load()
        {
            m_localizeData = new Dictionary<string, LocalizeDataItem>();
            if (Data != null && Data.LocalizeData.Count > 0)
            {
                m_localizeData.Clear();
                foreach (var dataItem in Data.LocalizeData)
                {
                    m_localizeData.TryAdd(dataItem.Key, dataItem);
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
                return m_localizeData[key].Data;
            }
            PDebug.WarnFormat("[BlueprintLocalize] Missing localize text of ID ({0})", key);
            return string.Empty;
        }
    }
}

