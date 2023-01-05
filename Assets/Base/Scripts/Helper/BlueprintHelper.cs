using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Base.Logging;
using Google.Protobuf;
using UnityEngine;

namespace Base.Helper
{
    public static class BlueprintHelper
    {
        public static T ProtoDeserialize<T>(byte[] rawData) where T : IMessage<T>
        {
            try
            {
                MessageParser<T> parser = (MessageParser<T>)typeof(T).GetProperty("Parser")?.GetValue(null, null);

                return parser.ParseFrom(rawData);
            }
            catch (Exception exception)
            {
                BaseLogSystem.GetLogger().Error("[ProtoDeserialized] Parse {0} Error {1}", typeof(T).Name, exception);

                throw;
            }
        }
        
        public static T ProtoDeserialize<T>(ByteString data) where T : IMessage<T>
        {
            try
            {
                MessageParser<T> parser = (MessageParser<T>)typeof(T).GetProperty("Parser").GetValue(null, null);
                return parser.ParseFrom(data);
            }
            catch (Exception e)
            {
                BaseLogSystem.GetLogger().Error("[ProtoDeserialized] Parse {0} Error {1}", typeof(T).Name, e);
                throw e;
            }
        }
        /// <summary>
        /// Deserialize a json string to proto class
        /// </summary>
        /// <param name="rawData">json string</param>
        /// <typeparam name="T">proto message</typeparam>
        /// <returns>proto message</returns>
        public static T ProtoDeserialize<T>(string rawData) where T : IMessage<T>
        {
            try
            {
                MessageParser<T> parser = (MessageParser<T>)typeof(T).GetProperty("Parser").GetValue(null, null);
                return parser.ParseJson(rawData);
            }
            catch (Exception e)
            {
                BaseLogSystem.GetLogger().Error("[ProtoDeserialized] Parse {0} Error {1}", typeof(T).Name, e);
                throw;
            }
        }

        public static T ProtoDeserialize<T>(Stream stream) where T : IMessage<T>
        {
            try
            {
                MessageParser<T> parser = (MessageParser<T>)typeof(T).GetProperty("Parser").GetValue(null, null);
                return parser.ParseFrom(stream);
            }
            catch (Exception e)
            {
                BaseLogSystem.GetLogger().Error("[ProtoDeserialized] Parse {0} Error {1}", typeof(T).Name, e);
                throw;
            }
        }
    }
}

