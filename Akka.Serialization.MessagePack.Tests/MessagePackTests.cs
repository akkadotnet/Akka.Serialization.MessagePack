//-----------------------------------------------------------------------
// <copyright file="MessagePackTests.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Serialization.MsgPack;
using Akka.Tests.Serialization;

namespace Akka.Serialization.MessagePack.Tests
{
    public class MessagePackTests : AkkaSerializationSpec
    {
        public MessagePackTests() : base(typeof(MsgPackSerializer))
        {
        }
    }
}