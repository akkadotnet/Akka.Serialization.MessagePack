using System;
using System.Collections.Generic;

namespace Akka.Serialization.MessagePack.Tests.TestKit
{
    public static class Messages
    {
        public class EmptyMessage { }

        public class EmptySingleton
        {
            public static EmptySingleton Instance { get; } = new EmptySingleton();

            private EmptySingleton() { }
        }

        public sealed class ContractlessSample
        {
            public int MyProperty1 { get; set; }
            public int MyProperty2 { get; set; }

            private bool Equals(ContractlessSample other)
            {
                return MyProperty1 == other.MyProperty1 && MyProperty2 == other.MyProperty2;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ContractlessSample && Equals((ContractlessSample)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (MyProperty1 * 397) ^ MyProperty2;
                }
            }
        }

        public sealed class ImmutableMessageWithReadonlyFields
        {
            public ImmutableMessageWithReadonlyFields(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public readonly string Name;

            public readonly int Age;

            private bool Equals(ImmutableMessageWithReadonlyFields other)
            {
                return string.Equals(Name, other.Name) && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithReadonlyFields && Equals((ImmutableMessageWithReadonlyFields)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
                }
            }
        }

        public sealed class ImmutableMessageWithSimpleTypes
        {
            public ImmutableMessageWithSimpleTypes(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; }

            public int Age { get; }

            private bool Equals(ImmutableMessageWithSimpleTypes other)
            {
                return String.Equals(Name, (string) other.Name) && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithSimpleTypes && Equals((ImmutableMessageWithSimpleTypes)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
                }
            }
        }

        public sealed class ImmutableMessageWithGenericTypes<T>
        {
            public ImmutableMessageWithGenericTypes(string name, T data)
            {
                Name = name;
                Data = data;
            }

            public string Name { get; }

            public T Data { get; }

            private bool Equals(ImmutableMessageWithGenericTypes<T> other)
            {
                return String.Equals(Name, (string) other.Name) && EqualityComparer<T>.Default.Equals(Data, other.Data);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithGenericTypes<T> && Equals((ImmutableMessageWithGenericTypes<T>)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ EqualityComparer<T>.Default.GetHashCode(Data);
                }
            }
        }

        public sealed class ImmutableMessageWithObjectTypes
        {
            public ImmutableMessageWithObjectTypes(string name, object data)
            {
                Name = name;
                Data = data;
            }

            public string Name { get; }

            public object Data { get; }

            private bool Equals(ImmutableMessageWithObjectTypes other)
            {
                return String.Equals(Name, (string) other.Name) && Equals(Data, other.Data);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithObjectTypes && Equals((ImmutableMessageWithObjectTypes)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Data != null ? Data.GetHashCode() : 0);
                }
            }
        }

        public sealed class ImmutableMessageWithTwoConstructors
        {
            public ImmutableMessageWithTwoConstructors(string name)
            {
                Name = name;
                Age = 500;
            }

            public ImmutableMessageWithTwoConstructors(int age, string name)
            {
                Name = name;
                Age = age;
            }

            public ImmutableMessageWithTwoConstructors(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; }

            public int Age { get; }

            private bool Equals(ImmutableMessageWithTwoConstructors other)
            {
                return String.Equals(Name, (string) other.Name) && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithTwoConstructors && Equals((ImmutableMessageWithTwoConstructors)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
                }
            }
        }

        public sealed class ImmutableMessageWithObjectsInsideObjects
        {
            public ImmutableMessageWithObjectsInsideObjects(object subObject)
            {
                SubObject = subObject;
            }

            public object SubObject { get; }

            private bool Equals(ImmutableMessageWithObjectsInsideObjects other)
            {
                return Equals(SubObject, other.SubObject);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithObjectsInsideObjects && Equals((ImmutableMessageWithObjectsInsideObjects)obj);
            }

            public override int GetHashCode()
            {
                return (SubObject != null ? SubObject.GetHashCode() : 0);
            }
        }
    }
}
