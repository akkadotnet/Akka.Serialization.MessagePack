//-----------------------------------------------------------------------
// <copyright file="MsgPackSerializersTests.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Serialization.Testkit;
using Xunit;

namespace Akka.Serialization.MessagePack.Tests
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

#if SERIALIZABLE
    public class MsgPackExceptionsTests : ExceptionsTests
    {
        public MsgPackExceptionsTests() : base(typeof(MsgPackSerializer))
        {
        }
    }
#endif

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

    public class MsgPackIncapsulationTests : IncapsulationTests
    {
        public MsgPackIncapsulationTests() : base(typeof(MsgPackSerializer))
        {
        }

        [Fact(Skip = "Not supported yet")]
        public override void Can_Serialize_a_class_with_internal_constructor()
        {
        }

        [Fact(Skip = "Not supported yet")]
        public override void Can_Serialize_a_class_with_private_constructor()
        {
        }

        [Fact(Skip = "Not supported yet")]
        public override void Can_Serialize_internal_class()
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
