using System.Collections.Generic;
using Base.Pattern;
using UnityEngine;
using System;
using System.Text;
using Base.Logging;

namespace Base.Services
{
    public class LanguageChangeSignal : Signal<string> {}
    
    public class LanguageChangedRequestSignal : Signal<string> {}

    public enum LanguageCode {En, Vi}
    
    public class LocalizeManager : IService
    {
        private readonly Dictionary<SystemLanguage, LanguageCode> _supportLanguage = new Dictionary<SystemLanguage, LanguageCode>
        {
            {SystemLanguage.English, LanguageCode.En},
            {SystemLanguage.Vietnamese, LanguageCode.Vi}
        };

        private string KeyLang = "paidrubik_lang";

        private LanguageCode _currentLang;

        public LanguageCode CurrentLanguage => _currentLang;
        
        public void Init()
        {
            ServiceLocator.SetSignal<LanguageChangedRequestSignal>();

            if (PlayerPrefs.HasKey(KeyLang))
            {
                if (Enum.TryParse(PlayerPrefs.GetString(KeyLang), out LanguageCode langCode))
                {
                    _currentLang = langCode;
                }
            }
            else if (_supportLanguage.ContainsKey(Application.systemLanguage))
            {
                _supportLanguage.TryGetValue(Application.systemLanguage, out _currentLang);
            }
            else
            {
                _currentLang = LanguageCode.En;
            }
        }

        public void DeInit()
        {
            PlayerPrefs.SetString(KeyLang, _currentLang.ToString());
            PlayerPrefs.Save();
        }

        public void SetLanguage(LanguageCode langCode)
        {
            bool isChanged = _currentLang != langCode;
            _currentLang = langCode;
            
            if (isChanged)
            {
                PlayerPrefs.SetString(KeyLang, _currentLang.ToString());
                PlayerPrefs.Save();

                try
                {
                    ServiceLocator.GetSignal<LanguageChangedRequestSignal>()?.Dispatch(_currentLang.ToString());
                }
                catch (Exception e)
                {
                    BaseLogSystem.GetLogger().Error(e);
                }
            }
        }
    }

    public static class Localization
    {
        private static string GetLocalizeText(string key)
        {
            BlueprintLocalization blueprint = ServiceLocator.GetService<BlueprintLocalization>();
            string text = blueprint?.GetTextByKey(key);
        
            return text != string.Empty ? text : key;
        }
        
        public static string GetText(string textID)
        {
            return GetLocalizeText(textID);
        }
        
        public static string Format(string textID, params object[] args)
        {
            return Format(null, GetLocalizeText(textID), args);
        }

        static string Format(IFormatProvider provider, string format, params object[] args)
        {
            if (format == null) { return null; }
            if (args == null) { throw new ArgumentNullException("args"); }

            StringBuilder sb = new StringBuilder(format.Length + args.Length * 8);
            sb.AppendFormat(provider, format, args);
            return sb.ToString();
        }
    }
}