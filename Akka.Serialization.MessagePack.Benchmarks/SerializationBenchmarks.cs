using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Configuration;
using Akka.Util.Internal;
using BenchmarkDotNet.Attributes;
using MessagePack;

namespace Akka.Serialization.MessagePack.Benchmarks
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public class SerializationBenchmarks
    {
        private MsgPackSerializer MsgPackSerializer;
        private HyperionSerializer HyperionSerializer;
        private NewtonSoftJsonSerializer NewtonSoftJsonSerializer;

        private const string TestString = "simple long string";

        private SimpleObject TestSimpleObject = new SimpleObject()
        {
            MyProperty1 = 1, MyProperty2 = 2, MyProperty3 = 3, MyProperty4 = 4, MyProperty5 = 5
        };

        private SimpleOptimizedObject TestSimpleObjectOptimized = new SimpleOptimizedObject
        {
            MyProperty1 = 1, MyProperty2 = 2, MyProperty3 = 3, MyProperty4 = 4, MyProperty5 = 5
        };

        private TypelessObject TestTypelessObject = new TypelessObject
        {
            MyProperty1 = Int16.MaxValue, MyProperty2 = Single.MaxValue, MyProperty3 = TestString, MyProperty4 = TimeSpan.FromDays(3), MyProperty5 = new Uri("http://somesite.com")
        };

        public SerializationBenchmarks()

        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true");
            var system = ActorSystem.Create("SerializationBenchmarks", config);

            MsgPackSerializer = new MsgPackSerializer(system.AsInstanceOf<ExtendedActorSystem>());
            HyperionSerializer = new HyperionSerializer(
                system.AsInstanceOf<ExtendedActorSystem>(), 
                new HyperionSerializerSettings(false, false, typeof(SimpleTypesProvider)));
            NewtonSoftJsonSerializer = new NewtonSoftJsonSerializer(system.AsInstanceOf<ExtendedActorSystem>());
        }

        [Benchmark]
        public string MsgPack_serialize_string()
        {
            var bytes = MsgPackSerializer.ToBinary(TestString);
            return MsgPackSerializer.FromBinary<string>(bytes);
        }

        [Benchmark]
        public string Hyperion_serialize_string()
        {
            var bytes = HyperionSerializer.ToBinary(TestString);
            return HyperionSerializer.FromBinary<string>(bytes);
        }

        [Benchmark]
        public string JsonNet_serialize_string()
        {
            var bytes = NewtonSoftJsonSerializer.ToBinary(TestString);
            return NewtonSoftJsonSerializer.FromBinary<string>(bytes);
        }

        [Benchmark]
        public SimpleObject MsgPack_serialize_SimpleObject()
        {
            var bytes = MsgPackSerializer.ToBinary(TestSimpleObject);
            return MsgPackSerializer.FromBinary<SimpleObject>(bytes);
        }

        [Benchmark]
        public SimpleObject Hyperion_serialize_SimpleObject()
        {
            var bytes = HyperionSerializer.ToBinary(TestSimpleObject);
            return HyperionSerializer.FromBinary<SimpleObject>(bytes);
        }

        [Benchmark]
        public SimpleObject JsonNet_serialize_SimpleObject()
        {
            var bytes = NewtonSoftJsonSerializer.ToBinary(TestSimpleObject);
            return NewtonSoftJsonSerializer.FromBinary<SimpleObject>(bytes);
        }

        [Benchmark]
        public SimpleOptimizedObject MsgPack_serialize_SimpleOptimizedObject_int_keys()
        {
            var bytes = MsgPackSerializer.ToBinary(TestSimpleObjectOptimized);
            return MsgPackSerializer.FromBinary<SimpleOptimizedObject>(bytes);
        }

        [Benchmark]
        public SimpleOptimizedObject Hyperion_serialize_SimpleOptimizedObject_preregistered()
        {
            var bytes = HyperionSerializer.ToBinary(TestSimpleObjectOptimized);
            return HyperionSerializer.FromBinary<SimpleOptimizedObject>(bytes);
        }

        [Benchmark]
        public TypelessObject MsgPack_serialize_TypelessObject()
        {
            var bytes = MsgPackSerializer.ToBinary(TestTypelessObject);
            return MsgPackSerializer.FromBinary<TypelessObject>(bytes);
        }

        [Benchmark]
        public TypelessObject Hyperion_serialize_TypelessObject()
        {
            var bytes = HyperionSerializer.ToBinary(TestTypelessObject);
            return HyperionSerializer.FromBinary<TypelessObject>(bytes);
        }

        [Benchmark]
        public TypelessObject JsonNet_serialize_TypelessObject()
        {
            var bytes = NewtonSoftJsonSerializer.ToBinary(TestTypelessObject);
            return NewtonSoftJsonSerializer.FromBinary<TypelessObject>(bytes);
        }
    }

    public class SimpleObject
    {
        public int MyProperty1 { get; set; }
        public int MyProperty2 { get; set; }
        public int MyProperty3 { get; set; }
        public int MyProperty4 { get; set; }
        public int MyProperty5 { get; set; }
    }

    [MessagePackObject]
    public class SimpleOptimizedObject
    {
        [Key(0)]
        public int MyProperty1 { get; set; }

        [Key(1)]
        public int MyProperty2 { get; set; }

        [Key(2)]
        public int MyProperty3 { get; set; }

        [Key(3)]
        public int MyProperty4 { get; set; }

        [Key(4)]
        public int MyProperty5 { get; set; }
    }

    public class TypelessObject
    {
        public object MyProperty1 { get; set; }
        public object MyProperty2 { get; set; }
        public object MyProperty3 { get; set; }
        public object MyProperty4 { get; set; }
        public object MyProperty5 { get; set; }
    }

    public class SimpleTypesProvider : IKnownTypesProvider
    {
        public SimpleTypesProvider(ExtendedActorSystem system)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));
        }

        public IEnumerable<Type> GetKnownTypes() => new Type[] { typeof(SimpleOptimizedObject) };
    }
}
