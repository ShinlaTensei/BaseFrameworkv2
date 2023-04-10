using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Logging;
using Google.Protobuf;
using Newtonsoft.Json;
using UnityEngine;

namespace Base.Module
{
    public interface IBlueprint
    {
        void InitBlueprint(bool usingLocal = false);
        void Load();
        void LoadDummyData();

        string TypeUrl     { get; set; }
        bool   IsDataReady { get; set; }
        bool   LoadDummy   { get; set; }
    }
    
    public interface IJsonDataDeserialize
    {
        void DeserializeJson(string json);

        string SerializeJson();
    }

    public interface IProtoDataDeserialize
    {
        void DeserializeProto(byte[] rawData);

        byte[] SerializeProto();
    }

    public abstract class BaseBlueprint<T> : IBlueprint, IJsonDataDeserialize, IProtoDataDeserialize where T : IMessage<T>
    {
        public string TypeUrl { get; set; }
        public bool IsDataReady { get; set; }
        public bool LoadDummy { get; set; }

        public T Data;
        public virtual void InitBlueprint(bool usingLocal = false)
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
            Data = BlueprintHelper.ProtoDeserialize<T>(json);
            IsDataReady = Data != null;
        }

        public virtual void DeserializeProto(byte[] rawData)
        {
            Data = BlueprintHelper.ProtoDeserialize<T>(rawData);
            IsDataReady = Data != null;
        }

        public virtual byte[] SerializeProto()
        {
            return Data != null ? Data.ToByteArray() : null;
        }

        public virtual string SerializeJson()
        {
            try
            {
                JsonFormatter formatter = new JsonFormatter(new JsonFormatter.Settings(true));
                string jsonString = formatter.Format(Data);
                object jsonObject = JsonConvert.DeserializeObject(jsonString);
                jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

                return jsonString;
            }
            catch (Exception exception)
            {
                PDebug.ErrorFormat("[{0}] Error: {1}", GetType(), exception);
            }

            return string.Empty;
        }
    }
}

