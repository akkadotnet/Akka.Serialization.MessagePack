//-----------------------------------------------------------------------
// <copyright file="MsgPackSerializerSettings.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using Akka.Configuration;

namespace Akka.Serialization.MessagePack
{
    public class MsgPackSerializerSettings
    {
        public MsgPackSerializerSettings(bool enableLz4Compression)
        {
            EnableLz4Compression = enableLz4Compression;
        }

        public bool EnableLz4Compression { get; }

        public static readonly MsgPackSerializerSettings Default = new MsgPackSerializerSettings(
            enableLz4Compression: false);

        public static MsgPackSerializerSettings Create(Config config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config), "MsgPackSerializerSettings require a config, default path: `akka.serializers.msgpack`");

            return new MsgPackSerializerSettings(
                enableLz4Compression: config.GetBoolean("enable-lz4-compression", false));
        }
    }
}
