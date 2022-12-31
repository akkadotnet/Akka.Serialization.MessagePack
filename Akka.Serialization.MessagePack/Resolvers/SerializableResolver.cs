//-----------------------------------------------------------------------
// <copyright file="SerializableResolver.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------
#if SERIALIZATION
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Akka.Util.Internal;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class SerializableResolver : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new SerializableResolver();
        SerializableResolver() { }

        public IMessagePackFormatter<T> GetFormatter<T>() => FormatterCache<T>.Formatter;

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;
            static FormatterCache() => Formatter = (IMessagePackFormatter<T>)SerializableFormatterHelper.GetFormatter<T>();
        }
    }

    internal static class SerializableFormatterHelper
    {
        internal static object GetFormatter<T>()
        {
            return typeof(Exception).IsAssignableFrom(typeof(T)) ? new SerializableFormatter<T>() : null;
        }
    }

    public class SerializableFormatter<T> : IMessagePackFormatter<T>
    {
        private static readonly IMessagePackFormatter<object> ObjectFormatter = TypelessFormatter.Instance;

        public void Serialize(ref MessagePackWriter writer, T value,
            MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            var serializable = value as ISerializable;
            var serializationInfo = new SerializationInfo(value.GetType(), new FormatterConverter());
            serializable.GetObjectData(serializationInfo, new StreamingContext());
            
            writer.WriteMapHeader(serializationInfo.MemberCount);
            foreach (var info in serializationInfo)
            {
                writer.WriteString(Encoding.UTF8.GetBytes(info.Name));
                ObjectFormatter.Serialize(ref writer, info.Value, options);
            }
        }

        public T Deserialize(ref MessagePackReader reader,
            MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                reader.ReadNil();
                return default(T);
            }

            var serializationInfo = new SerializationInfo(typeof(T), new FormatterConverter());

            var len = reader.ReadMapHeader();
            for (int i = 0; i < len; i++)
            {
                var key = reader.ReadString();
                var val = ObjectFormatter.Deserialize(ref reader,options);
                serializationInfo.AddValue(key, val);
            }

            ISerializable obj = null;
            ConstructorInfo constructorInfo = typeof(T).GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(SerializationInfo), typeof(StreamingContext) },
                null);

            if (constructorInfo != null)
            {
                object[] args = { serializationInfo, new StreamingContext() };
                obj = constructorInfo.Invoke(args).AsInstanceOf<ISerializable>();
            }
            return (T)obj;
        }
    }
}
#endif