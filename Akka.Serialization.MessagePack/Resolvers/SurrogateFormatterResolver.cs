﻿//-----------------------------------------------------------------------
// <copyright file="AkkaResolver.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Util.Internal;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class SurrogateFormatterResolver : IFormatterResolver
    {

        private readonly ConcurrentDictionary<Type, IMessagePackFormatter>
            _formatterCache =
                new ConcurrentDictionary<Type, IMessagePackFormatter>();

        private readonly Func<Type, IMessagePackFormatter> _formatterCreateFunc;
        public SurrogateFormatterResolver(ExtendedActorSystem system)
        {
            //Cast in the func since we'll have to cache anyway.
            //The alternative is making a 'nullable' func in another static class,
            //But that may result in too much garbage for other types.
            _formatterCreateFunc = t =>
                (IMessagePackFormatter)typeof(SurrogateFormatter<>)
                    .MakeGenericType(t)
                    .GetConstructor(new[] { typeof(ActorSystem) })
                    .Invoke(new[] { system });

        }
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (SurrogateResolvable<T>.IsSurrogate)
            {
                return (IMessagePackFormatter<T>)_formatterCache.GetOrAdd(
                    typeof(T),
                    _formatterCreateFunc);
            }
            else
            {
                return null;
            }
        }
        
    }
}
