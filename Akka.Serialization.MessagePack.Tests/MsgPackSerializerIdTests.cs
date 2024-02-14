// -----------------------------------------------------------------------
//   <copyright file="MsgPackSerializerOverrideTests.cs" company="Petabridge, LLC">
//     Copyright (C) 2015-2024 .NET Petabridge, LLC
//   </copyright>
// -----------------------------------------------------------------------

using Akka.Serialization.Testkit.Util;
using FluentAssertions;
using Xunit;

namespace Akka.Serialization.MessagePack.Tests;

public class MsgPackSerializerIdTests: TestKit.Xunit2.TestKit
{
    public MsgPackSerializerIdTests() : base(ConfigFactory.GetConfig(typeof(MsgPackSerializer), true))
    {
    }

    [Fact(DisplayName = "Must be able to override serializer id from HOCON settings")]
    public void OverridenIdTest()
    {
        var serializer = Sys.Serialization.FindSerializerForType(typeof(object));
        serializer.Should().BeOfType<MsgPackSerializer>();
        serializer.Identifier.Should().Be(ConfigFactory.SerializerIdOverride);
    }
}

public class MsgPackDefaultSerializerIdTests: TestKit.Xunit2.TestKit
{
    public MsgPackDefaultSerializerIdTests() : base(ConfigFactory.GetConfig(typeof(MsgPackSerializer)))
    {
    }

    [Fact(DisplayName = "Default serializer ID must be the default value")]
    public void OverridenIdTest()
    {
        var serializer = Sys.Serialization.FindSerializerForType(typeof(object));
        serializer.Should().BeOfType<MsgPackSerializer>();
        serializer.Identifier.Should().Be(MsgPackSerializer.DefaultSerializerId);
    }
}