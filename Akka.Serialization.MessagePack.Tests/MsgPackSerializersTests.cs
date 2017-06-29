using System;
using Akka.Serialization.MessagePack;
using Akka.Serialization.Testkit;
using Xunit;

namespace Akka.Serialization.MsgPack.Tests
{
    public class MsgPackAkkaMessagesTests : AkkaMessagesTests
    {
        public MsgPackAkkaMessagesTests() : base(typeof(MsgPackSerializer))
        {
        }
    }

    public class MsgPackCollectionsTests : CollectionsTests
    {
        public MsgPackCollectionsTests() : base(typeof(MsgPackSerializer))
        {
        }
    }

    public class MsgPackCustomMessagesTests : CustomMessagesTests
    {
        public MsgPackCustomMessagesTests() : base(typeof(MsgPackSerializer))
        {
        }
    }

    public class MsgPackExceptionsTests : ExceptionsTests
    {
        public MsgPackExceptionsTests() : base(typeof(MsgPackSerializer))
        {
        }
    }

    public class MsgPackImmutableMessagesTests : ImmutableMessagesTests
    {
        public MsgPackImmutableMessagesTests() : base(typeof(MsgPackSerializer))
        {
        }
    }

    public class MsgPackPolymorphismTests : PolymorphismTests
    {
        public MsgPackPolymorphismTests() : base(typeof(MsgPackSerializer))
        {
        }
    }

    public class MsgPackPrimiviteSerializerTests : PrimitiveSerializerTests
    {
        public MsgPackPrimiviteSerializerTests() : base(typeof(MsgPackSerializer))
        {
        }
    }
}
