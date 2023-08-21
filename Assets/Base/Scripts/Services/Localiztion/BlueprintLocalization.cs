#region Header
// Date: 04/08/2023
// Created by: Huynh Phong Tran
// File name: BlueprintLocalization.cs
#endregion

using System.Collections;
using System.Collections.Generic;
using Base.Logging;
using Base.Module;
using Base.Pattern;
using FileHelpers;
using UnityEngine;

namespace Base.Services
{
    [DelimitedRecord(","), IgnoreFirst]
    public record LocalizeRecord
    {
        public string Key { get; set; }
        public string VI  { get; set; }
        public string EN  { get; set; }
    }
    
    [BlueprintReader("Data/Localize", DataFormat.Csv, RemotePath = "BlueprintLocalization")]
    public class BlueprintLocalization : BaseBlueprintCsv<LocalizeRecord>
    {
        private IDictionary<string, string> m_localizeData = new Dictionary<string, string>();
        
        private const string LocalizePath = "Data/Localize";
        private       bool   IsFireSignal { get; set; }
        
        public override void Load()
        {
            if (Data is null || Data.Count < 0) { return; }
            
            LanguageCode code = ServiceLocator.Get<LocalizeManager>()!.CurrentLanguage;
            for (int i = 0; i < Data.Count; ++i)
            {
                InitLocalizeData(code, Data[i]);
            }

            if (IsFireSignal)
            {
                ServiceLocator.Get<LanguageChangedRequestSignal>()?.Dispatch(code.ToString());
            }
        }

        public override void LoadDummyData()
        {
            return;
        }

        public IEnumerator LoadLocalText()
        {
            ResourceRequest request = Resources.LoadAsync<TextAsset>(LocalizePath);

            yield return new WaitUntil(() => request.isDone);
            
            PDebug.InfoFormat("[BlueprintLocalize] Load local file {0}", request.asset);
            
            if (request.asset is TextAsset textAsset)
            {
                DeserializeCsv(textAsset.text);
                IsFireSignal = false;
                Load();
                IsFireSignal = true;
            }
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

        private void InitLocalizeData(LanguageCode langCode, LocalizeRecord @record)
        {
            switch (langCode)
            {
                case LanguageCode.En:
                    m_localizeData[@record.Key] = record.EN;
                    break;
                case LanguageCode.Vi:
                    m_localizeData[record.Key] = record.VI;
                    break;
            }
        }
    }
}
