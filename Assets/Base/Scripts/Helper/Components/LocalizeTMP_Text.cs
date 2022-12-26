using System;
using Base.Pattern;
using Base.Services;
using TMPro;
using UnityEngine;

namespace Base.Helper
{
    public class LocalizeTMP_Text : BaseMono
    {
        [SerializeField] private string mainKey;
        private TMP_Text _tmpText;
        private bool _isInit = false;

        private void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
            ServiceLocator.GetSignal<LanguageChangeSignal>()?.Subscribe(OnLanguageChanged);
        }

        protected override void Start()
        {
            base.Start();
            
            if (!_isInit) SetText();
        }

        private void OnDestroy()
        {
            ServiceLocator.GetSignal<LanguageChangeSignal>()?.UnSubscribe(OnLanguageChanged);
        }

        private void OnLanguageChanged(string langCode)
        {
            SetText();
        }

        private void SetText()
        {
            _tmpText.text = Localize.GetText(mainKey);
            _isInit = true;
        }
    }
}