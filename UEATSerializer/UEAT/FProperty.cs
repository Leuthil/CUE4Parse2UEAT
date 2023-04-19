using UEATSerializer.Serializer;
using Newtonsoft.Json;
using UEATSerializer.UE;

namespace UEATSerializer.UEAT
{
    public class FProperty : IUObjectPointer, ISerializableForUEAT
    {
        public string ObjectClass { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;
        public int ArrayDim { get; set; }

        /// <summary>
        /// FProperty's EPropertyFlags value. Automatically serializes ONLY flags that are not computed (& ~CPF_ComputedFlags).
        /// </summary>
        public ulong PropertyFlags { get; set; } = (ulong)EPropertyFlags.CPF_None;

        // RepNotifyFunc.ToString()
        public string RepNotifyFunc { get; set; } = string.Empty;

        // ELifetimeCondition
        public int BlueprintReplicationCondition { get; set; }

        public virtual int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            return Array.Empty<int>();
        }

        public virtual void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartObject();
            WriteJsonInlined(writer, serializer, objectHierarchy);
            writer.WriteEndObject();
        }

        public virtual void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WritePropertyName("ObjectClass");
            writer.WriteValue(ObjectClass);
            writer.WritePropertyName("ObjectName");
            writer.WriteValue(ObjectName);
            writer.WritePropertyName("ArrayDim");
            writer.WriteValue(ArrayDim);
            writer.WritePropertyName("PropertyFlags");
            // for some reason this should be serialized as a string
            writer.WriteValue((PropertyFlags & (ulong)~EPropertyFlags.CPF_ComputedFlags).ToString());
            writer.WritePropertyName("RepNotifyFunc");
            writer.WriteValue(RepNotifyFunc);
            writer.WritePropertyName("BlueprintReplicationCondition");
            writer.WriteValue(BlueprintReplicationCondition);
        }
    }

    public class FEnumProperty : FProperty
    {
        // EnumProperty->GetEnum() (UEnum type)
        public PackageObject Enum { get; set; }
        public FNumericProperty UnderlyingProp { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            var referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));
            referencedObjects.Add(objectHierarchy.AddUnique(Enum));

            if (Enum != null)
            {
                referencedObjects.UnionWith(Enum.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("Enum");
            writer.WriteValue(objectHierarchy.GetObjectIndex(Enum?.Id));
            writer.WritePropertyName("UnderlyingProp");
            UnderlyingProp.WriteJson(writer, serializer, objectHierarchy);
        }
    }

    public class FStructProperty : FProperty
    {
        // StructProperty->Struct (UScriptStruct)
        public PackageObject Struct { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));
            referencedObjects.Add(objectHierarchy.AddUnique(Struct));

            if (Struct != null)
            {
                referencedObjects.UnionWith(Struct.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("Struct");
            writer.WriteValue(objectHierarchy.GetObjectIndex(Struct?.Id));
        }
    }

    public class FArrayProperty : FProperty
    {
        // FArrayProperty->Inner (FProperty)
        public FProperty Inner { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));

            if (Inner != null)
            {
                referencedObjects.UnionWith(Inner.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("Inner");
            Inner.WriteJson(writer, serializer, objectHierarchy);
        }
    }

    public class FBoolProperty : FProperty
    {
        // ElementSize
        public int BoolSize { get; set; }
        public bool NativeBool { get; set; }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("BoolSize");
            writer.WriteValue(BoolSize);
            writer.WritePropertyName("NativeBool");
            writer.WriteValue(NativeBool);
        }
    }

    // Enum for legacy enum properties (TByteAsEnum)
    public class FByteProperty : FProperty
    {
        // ByteProperty->Enum (UEnum)
        public PackageObject Enum { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));
            referencedObjects.Add(objectHierarchy.AddUnique(Enum));

            if (Enum != null)
            {
                referencedObjects.UnionWith(Enum.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("Enum");
            writer.WriteValue(objectHierarchy.GetObjectIndex(Enum?.Id));
        }
    }

    public class FClassProperty : FProperty
    {
        // ClassProperty->MetaClass (UClass)
        public PackageObject MetaClass { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));
            referencedObjects.Add(objectHierarchy.AddUnique(MetaClass));

            if (MetaClass != null)
            {
                referencedObjects.UnionWith(MetaClass.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("MetaClass");
            writer.WriteValue(objectHierarchy.GetObjectIndex(MetaClass?.Id));
        }
    }

    public class FInterfaceProperty : FProperty
    {
        // InterfaceProperty->InterfaceClass (UClass)
        public PackageObject InterfaceClass { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));
            referencedObjects.Add(objectHierarchy.AddUnique(InterfaceClass));

            if (InterfaceClass != null)
            {
                referencedObjects.UnionWith(InterfaceClass.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("InterfaceClass");
            writer.WriteValue(objectHierarchy.GetObjectIndex(InterfaceClass?.Id));
        }
    }

    public class FMapProperty : FProperty
    {
        public FProperty KeyProp { get; set; }
        public FProperty ValueProp { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));
            referencedObjects.UnionWith(KeyProp.ResolveObjectReferences(objectHierarchy));
            referencedObjects.UnionWith(ValueProp.ResolveObjectReferences(objectHierarchy));

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("KeyProp");
            KeyProp.WriteJson(writer, serializer, objectHierarchy);
            writer.WritePropertyName("ValueProp");
            ValueProp.WriteJson(writer, serializer, objectHierarchy);
        }
    }

    public class FSetProperty : FProperty
    {
        public FProperty ElementType { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));
            referencedObjects.UnionWith(ElementType.ResolveObjectReferences(objectHierarchy));

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("ElementType");
            ElementType.WriteJson(writer, serializer, objectHierarchy);
        }
    }

    public class FSoftClassProperty : FProperty
    {
        // SoftClassProperty->MetaClass (UClass)
        public PackageObject MetaClass { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));
            referencedObjects.Add(objectHierarchy.AddUnique(MetaClass));

            if (MetaClass != null)
            {
                referencedObjects.UnionWith(MetaClass.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("MetaClass");
            writer.WriteValue(objectHierarchy.GetObjectIndex(MetaClass?.Id));
        }
    }

    public class FFieldPathProperty : FProperty
    {
        // Serialize field class (FFieldClass)
        // FieldPathProperty->PropertyClass->GetName()
        public string PropertyClass { get; set; }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("PropertyClass");
            writer.WriteValue(PropertyClass);
        }
    }

    public class FObjectPropertyBase : FProperty
    {
        // ObjectProperty->PropertyClass (UClass)
        public PackageObject PropertyClass { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));
            referencedObjects.Add(objectHierarchy.AddUnique(PropertyClass));

            if (PropertyClass != null)
            {
                referencedObjects.UnionWith(PropertyClass.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("PropertyClass");
            writer.WriteValue(objectHierarchy.GetObjectIndex(PropertyClass?.Id));
        }
    }

    public class FDelegateProperty : FProperty
    {
        // For delegate properties, we need to serialize signature function
        // Since it will always be present in the Child array too, we serialize just it's name
        // and not actual full UFunction object
        // DelegateProperty->SignatureFunction->GetName()
        public string SignatureFunction { get; set; }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("SignatureFunction");
            writer.WriteValue(SignatureFunction);
        }
    }

    public class FMulticastDelegateProperty : FProperty
    {
        // For multicast delegate properties, record signature function name
        // MulticastDelegateProperty->SignatureFunction->GetName()
        public string SignatureFunction { get; set; }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("SignatureFunction");
            writer.WriteValue(SignatureFunction);
        }
    }

    // only declared so it can be referenced by other FProperty definitions
    public class FNumericProperty : FProperty
    {

    }

    // other property classes do not override serialize, so no need
}
