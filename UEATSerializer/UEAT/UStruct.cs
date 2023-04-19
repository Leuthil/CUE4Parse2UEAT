using UEATSerializer.Serializer;
using Newtonsoft.Json;

namespace UEATSerializer.UEAT
{
    public class UStruct : UObject
    {
        public PackageObject SuperStruct { get; set; }
        public List<UFunction> Children { get; set; } = new List<UFunction>();
        public Dictionary<string, FProperty> ChildProperties { get; set; } = new Dictionary<string, FProperty>();

        // (requires bytecode disassembly, not sure if available) (BytecodeDisassembler.SerializeFunction(struct))
        //public List<object> Script { get; set; } = new List<object>();

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));
            referencedObjects.Add(objectHierarchy.AddUnique(SuperStruct));

            if (SuperStruct != null)
            {
                referencedObjects.UnionWith(SuperStruct.ResolveObjectReferences(objectHierarchy));
            }

            foreach (var childProp in ChildProperties)
            {
                if (childProp.Value != null)
                {
                    referencedObjects.UnionWith(childProp.Value.ResolveObjectReferences(objectHierarchy));
                }
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WritePropertyName("SuperStruct");
            writer.WriteValue(objectHierarchy.GetObjectIndex(SuperStruct?.Id));

            writer.WritePropertyName("Children");
            writer.WriteStartArray();
            foreach (var child in Children)
            {
                if (child == null)
                {
                    continue;
                }

                writer.WriteStartObject();
                writer.WritePropertyName("FieldKind");
                writer.WriteValue("Function");
                child.WriteJsonInlined(writer, serializer, objectHierarchy);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WritePropertyName("ChildProperties");
            writer.WriteStartArray();
            foreach (var childProp in ChildProperties)
            {
                if (childProp.Value == null)
                {
                    continue;
                }

                writer.WriteStartObject();
                writer.WritePropertyName("FieldKind");
                writer.WriteValue("Property");
                childProp.Value.WriteJsonInlined(writer, serializer, objectHierarchy);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            base.WriteJsonInlined(writer, serializer, objectHierarchy);
        }
    }
}
