using System;
using System.Collections;
using System.Collections.Generic;
using Base.Logging;
using Base.Module;
using Base.Pattern;
using Base.Data.Structure;

namespace Base.Services
{
    public class BlueprintLocalization : BaseBlueprint<LocalizeDataStructure>, IService<List<LocalizeDataItem>>
    {
        private Dictionary<string, LocalizeDataItem> _localizeData;
        public void UpdateData(List<LocalizeDataItem> data)
        {
            if (data is {Count: > 0})
            {
                _localizeData.Clear();
                foreach (var dataItem in data)
                {
                    _localizeData.TryAdd(dataItem.Key, dataItem);
                }
            }
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

