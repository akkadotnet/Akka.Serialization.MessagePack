﻿//-----------------------------------------------------------------------
// <copyright file="ConfigFactory.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Akka.Serialization.Testkit.Util
{
    public class ConfigFactory
    {
        public static string GetConfig(Type serializerType)
        {
            return @"
                akka.actor {
                    serializers {
                        testserializer = """ + serializerType.AssemblyQualifiedName + @"""
                    }
                    serialization-bindings {
                      ""System.Object"" = testserializer
                    }
                }";
        }
    }
}
