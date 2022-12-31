//-----------------------------------------------------------------------
// <copyright file="AkkaResolver.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Util;
using Akka.Util.Internal;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{

    static class SurrogateResolvable<T>
    {
        public static readonly bool IsSurrogate = (typeof(ISurrogated).IsAssignableFrom(typeof(T)));
    }
    public class SurrogateFormatterResolver : IFormatterResolver
    {
        private readonly ActorSystem _system;

        private ConcurrentDictionary<Type, IMessagePackFormatter>
            _formatterCache =
                new ConcurrentDictionary<Type, IMessagePackFormatter>();

        private ConcurrentDictionary<Type, bool> _isResolvableCache =
            new ConcurrentDictionary<Type, bool>();

        private readonly Func<Type, IMessagePackFormatter> _formatterCreateFunc;
        public SurrogateFormatterResolver(ActorSystem system)
        {
            _system = system;
            _formatterCreateFunc = t =>
                (IMessagePackFormatter)typeof(SurrogateFormatter<>)
                    .MakeGenericType(t)
                    .GetConstructor(new[] { typeof(ActorSystem) })
                    .Invoke(new[] { _system });

        }
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            
            //if (typeof(ISurrogated).IsAssignableFrom(typeof(T)))
            if (SurrogateResolvable<T>.IsSurrogate)
            {
                return (IMessagePackFormatter<T>)_formatterCache.GetOrAdd(
                    typeof(T),
                    _formatterCreateFunc);
                //(k) => (IMessagePackFormatter)typeof(SurrogateFormatter<>)
                //    .MakeGenericType(k)
                //    .GetConstructor(new[] { typeof(ActorSystem) })
                //    .Invoke(new[] { _system }));
            }
            else
            {
                return null;
            }
        }
        
    }

    public class SurrogateFormatter<T> : IMessagePackFormatter<T>
        where T : ISurrogated
    {
        private readonly ActorSystem _system;

        private readonly ConcurrentDictionary<Type, IMessagePackFormatter>
            _dynamicFormatterDict = new ConcurrentDictionary<Type, IMessagePackFormatter>();
        public SurrogateFormatter(ActorSystem system)
        {
            _system = system;
        }


        public void Serialize(ref MessagePackWriter writer, T value,
            MessagePackSerializerOptions options)
        {
            options.Resolver.GetFormatter<ISurrogate>()
                .Serialize(ref writer, value.ToSurrogate(_system), options);
        }

        public T Deserialize(ref MessagePackReader reader,
            MessagePackSerializerOptions options)
        {
            var surrogate = options.Resolver.GetFormatter<ISurrogate>()
                .Deserialize(ref reader, options);
            return (T)surrogate.FromSurrogate(_system);
        }
    }
}
