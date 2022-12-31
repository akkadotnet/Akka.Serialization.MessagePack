﻿//-----------------------------------------------------------------------
// <copyright file="SerializableResolver.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------
#if SERIALIZATION
using System;
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
}
#endif