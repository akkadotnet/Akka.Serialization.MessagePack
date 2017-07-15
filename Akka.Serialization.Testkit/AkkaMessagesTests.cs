//-----------------------------------------------------------------------
// <copyright file="AkkaMessagesTests.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using Akka.Actor;
using Akka.Serialization.Testkit.Util;
using Akka.TestKit.TestActors;
using Xunit;

namespace Akka.Serialization.Testkit
{
    public abstract class AkkaMessagesTests : TestKit.Xunit2.TestKit
    {
        protected AkkaMessagesTests(Type serializerType) : base(ConfigFactory.GetConfig(serializerType))
        {
        }

        [Fact]
        public void Can_serialize_ActorRef()
        {
            var actorRef = ActorOf<BlackHoleActor>();
            AssertEqual(actorRef);
        }

        [Fact]
        public void Can_serialize_RootActorPath()
        {
            var uri = "akka.tcp://sys@localhost:9000";
            var actorPath = ActorPath.Parse(uri);
            AssertEqual(actorPath);
        }

        [Fact]
        public void Can_serialize_ActorPath()
        {
            var uri = "akka.tcp://sys@localhost:9000/user/actor";
            var actorPath = ActorPath.Parse(uri);
            AssertEqual(actorPath);
        }

        protected T AssertAndReturn<T>(T message)
        {
            var serializer = Sys.Serialization.FindSerializerFor(message);
            var serialized = serializer.ToBinary(message);
            var result = serializer.FromBinary(serialized, typeof(T));
            return (T)result;
        }

        protected void AssertEqual<T>(T message)
        {
            var deserialized = AssertAndReturn(message);
            Assert.Equal(message, deserialized);
        }
    }
}
