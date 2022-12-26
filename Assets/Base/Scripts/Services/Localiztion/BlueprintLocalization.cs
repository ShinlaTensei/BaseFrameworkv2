using System;
using System.Collections;
using System.Collections.Generic;
using Base.Logging;
using Base.Module;
using Base.Pattern;
using UnityEngine;

namespace Base.Services
{
    [Serializable]
    public class LocalizeDataItem
    {
        public string Key;
        public string Data;
    }

    /// <summary>
    /// The data-class get from server stand for a set of localize text for a language
    /// </summary>
    [Serializable]
    public class LocalizeDataStructure : IBlueprintData
    {
        public string LangCode;
        public List<LocalizeDataItem> LocalizeData = new List<LocalizeDataItem>();
    }

    public class BlueprintLocalization : BaseBlueprint<LocalizeDataStructure>, IService<LocalizeDataItem>
    {
        private Dictionary<string, LocalizeDataItem> _localizeData;
        public void UpdateData(LocalizeDataItem data)
        {
            
        }

        public void Init()
        {
            _localizeData = new Dictionary<string, LocalizeDataItem>();
        }

        public void DeInit()
        {
            
        }

        public override void Load()
        {
            if (Data != null && Data.LocalizeData.Count > 0)
            {
                _localizeData.Clear();
                foreach (var dataItem in Data.LocalizeData)
                {
                    _localizeData.TryAdd(dataItem.Key, dataItem);
                }
            }
        }

        public override void LoadDummyData()
        {
            throw new NotImplementedException();
        }

        public string GetTextByKey(string key)
        {
            if (_localizeData.ContainsKey(key))
            {
                return _localizeData[key].Data;
            }
            BaseLogSystem.GetLogger().Warn("[BlueprintLocalize] Missing localize text of ID ({0})", key);
            return string.Empty;
        }
    }
}

