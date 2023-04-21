using UEATSerializer.Serializer;
using Newtonsoft.Json;

namespace UEATSerializer.UEAT
{
    public abstract class FPropertyValue : IUObjectPointer, ISerializableForUEAT
    {
        public virtual int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            return Array.Empty<int>();
        }

        public abstract void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy);

        public virtual void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy) { 
            WriteJson(writer, serializer, objectHierarchy);
        }
    }

    public class FPropertyArrayValue : FPropertyValue
    {
        public List<FPropertyValue> Items { get; set; } = new List<FPropertyValue>();

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            foreach (var item in Items)
            {
                referencedObjects.UnionWith(item.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartArray();
            foreach (var item in Items)
            {
                item.WriteJson(writer, serializer, objectHierarchy);
            }
            writer.WriteEndArray();
        }
    }

    public class FMapPropertyValue : FPropertyValue
    {
        public Dictionary<FPropertyValue, FPropertyValue> KeyValuesPairs { get; set; } = new Dictionary<FPropertyValue, FPropertyValue>();

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            foreach (var keyValuePair in KeyValuesPairs)
            {
                referencedObjects.UnionWith(keyValuePair.Key.ResolveObjectReferences(objectHierarchy));
                referencedObjects.UnionWith(keyValuePair.Value.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartArray();
            foreach (var keyValuePair in KeyValuesPairs)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("Key");
                keyValuePair.Key.WriteJson(writer, serializer, objectHierarchy);
                writer.WritePropertyName("Value");
                keyValuePair.Value.WriteJson(writer, serializer, objectHierarchy);

                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }

    public class FSetPropertyValue : FPropertyValue
    {
        public List<FPropertyValue> Values { get; set; } = new List<FPropertyValue>();

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            foreach (var value in Values)
            {
                referencedObjects.UnionWith(value.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartArray();
            foreach (var value in Values)
            {
                value.WriteJson(writer, serializer, objectHierarchy);
            }
            writer.WriteEndArray();
        }
    }

    public class FArrayPropertyValue : FPropertyValue
    {
        public List<FPropertyValue> Values { get; set; } = new List<FPropertyValue>();

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            foreach (var value in Values)
            {
                referencedObjects.UnionWith(value.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartArray();
            foreach (var value in Values)
            {
                value.WriteJson(writer, serializer, objectHierarchy);
            }
            writer.WriteEndArray();
        }
    }

    public class FInterfacePropertyValue : FPropertyValue
    {
        public PackageObject Object { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.Add(objectHierarchy.AddUnique(Object));

            if (Object != null)
            {
                referencedObjects.UnionWith(Object.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue(objectHierarchy.GetObjectIndex(Object?.Id));
        }
    }

    public class FObjectPropertyBaseValue : FPropertyValue
    {
        public PackageObject Object { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.Add(objectHierarchy.AddUnique(Object));

            if (Object != null)
            {
                referencedObjects.UnionWith(Object.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue(objectHierarchy.GetObjectIndex(Object?.Id));
        }
    }

    public class FSoftObjectPropertyValue : FPropertyValue
    {
        public string Path { get; set; } = string.Empty;

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue(Path);
        }
    }

    public class FNumericPropertyValue : FPropertyValue
    {
        /// <summary>
        /// Use this if the value is a floating point number.
        /// </summary>
        public double? DoubleValue { get; set; } = null;

        /// <summary>
        /// Use this if the value is a signed integer number.
        /// </summary>
        public long? LongValue { get; set; } = null;

        /// <summary>
        /// Use this if the value is an unsigned integer number.
        /// </summary>
        public ulong? ULongValue { get; set; } = null;

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            if (DoubleValue.HasValue)
            {
                writer.WriteValue(DoubleValue.Value);
            }
            else if (LongValue.HasValue)
            {
                writer.WriteValue(LongValue.Value);
            }
            else if (ULongValue.HasValue)
            {
                writer.WriteValue(ULongValue.Value);
            }
            else
            {
                writer.WriteValue(0);
            }
        }
    }

    public class FBoolPropertyValue : FPropertyValue
    {
        public bool Value { get; set; }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue(Value);
        }
    }

    public class FStrPropertyValue : FPropertyValue
    {
        public string Value { get; set; } = string.Empty;

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue(Value);
        }
    }

    public class FEnumPropertyValue : FPropertyValue
    {
        /// <summary>
        /// UEnum FName as string
        /// </summary>
        public string Value { get; set; } = string.Empty;

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue(Value);
        }
    }

    public class FNamePropertyValue : FPropertyValue
    {
        public string Name { get; set; } = string.Empty;

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue(Name);
        }
    }

    public class FTextPropertyValue : FPropertyValue
    {
        public string Value { get; set; } = string.Empty;

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue(Value);
        }
    }

    public class FFieldPathPropertyValue : FPropertyValue
    {
        /// <summary>
        /// FFieldPath->ToString()
        /// </summary>
        public string Value { get; set; } = string.Empty;

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue(Value);
        }
    }

    /// <summary>
    /// DO NOT USE. Use FEnumProperty or FNumericProperty instead.
    /// </summary>
    [Obsolete("Use FEnumProperty or FNumericProperty instead", true)]
    public class FBytePropertyValue : FPropertyValue
    {
        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Not supported; serialized as string constant "##NOT SERIALIZED##"
    /// </summary>
    public class FMulticastDelegatePropertyValue : FPropertyValue
    {
        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue("##NOT SERIALIZED##");
        }
    }

    /// <summary>
    /// Not supported; serialized as string constant "##NOT SERIALIZED##"
    /// </summary>
    public class FDelegatePropertyValue : FPropertyValue
    {
        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue("##NOT SERIALIZED##");
        }
    }

    public class UnsupportedPropertyValue : FPropertyValue
    {
        public Dictionary<string, string> Data = new Dictionary<string, string>();

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteValue("##NOT SERIALIZED##");
        }
    }
}
