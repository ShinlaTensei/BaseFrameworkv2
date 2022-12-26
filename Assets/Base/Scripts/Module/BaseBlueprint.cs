using System;
using System.Collections;
using System.Collections.Generic;
using Base.Logging;
using Newtonsoft.Json;
using UnityEngine;

namespace Base.Module
{
    public interface IBlueprint
    {
        void Init(bool usingLocal = false);
        void Load();
        void LoadDummyData();

        string TypeUrl     { get; set; }
        bool   IsDataReady { get; set; }
        bool   LoadDummy   { get; set; }
    }
    
    public interface IJsonDataDeserialize
    {
        void DeserializeJson(string json);

        string SerializeObject();
    }

    public interface IBlueprintData
    {
        
    }
    
    public abstract class BaseBlueprint<T> : IBlueprint, IJsonDataDeserialize where T : IBlueprintData
    {
        public string TypeUrl { get; set; }
        public bool IsDataReady { get; set; }
        public bool LoadDummy { get; set; }

        public T Data;
        public void Init(bool usingLocal = false)
        {
            LoadDummy = usingLocal;
            if (usingLocal)
            {
                LoadDummyData();
            }
            else Load();
        }

        public abstract void Load();

        public abstract void LoadDummyData();
        public virtual void DeserializeJson(string json)
        {
            try
            {
                Data = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception exception)
            {
                BaseLogSystem.GetLogger().Error("[{0}] Error: {1}", GetType(), exception);
            }
        }

        public virtual string SerializeObject()
        {
            try
            {
                return JsonConvert.SerializeObject(Data);
            }
            catch (Exception exception)
            {
                BaseLogSystem.GetLogger().Error("[{0}] Error: {1}", GetType(), exception);
            }

            return string.Empty;
        }
    }
}

