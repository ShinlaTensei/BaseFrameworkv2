using System.Collections;
using System.Collections.Generic;
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
            Data = JsonConvert.DeserializeObject<T>(json);
        }

        public virtual string SerializeObject()
        {
            return JsonConvert.SerializeObject(Data);
        }
    }
}

