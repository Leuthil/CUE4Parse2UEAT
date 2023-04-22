using Newtonsoft.Json;
using UEATSerializer.Serializer;

namespace UEATSerializer.UEAT
{
    public class FImplementedInterface : IUObjectPointer, ISerializableForUEAT
    {
        public PackageObject Class { get; set; }
        public int PointerOffset { get; set; }
        public bool bImplementedByK2 { get; set; }

        public virtual int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.Add(objectHierarchy.AddUnique(Class));

            if (Class != null)
            {
                referencedObjects.UnionWith(Class.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public virtual void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartObject();
            WriteJsonInlined(writer, serializer, objectHierarchy);
            writer.WriteEndObject();
        }

        public virtual void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WritePropertyName("Class");
            writer.WriteValue(objectHierarchy.GetObjectIndex(Class?.Id));
            writer.WritePropertyName("PoinerOffset");
            writer.WriteValue(PointerOffset);
            writer.WritePropertyName("bImplementedByK2");
            writer.WriteValue(bImplementedByK2);
        }
    }
}
