using UEATSerializer.Serializer;
using Newtonsoft.Json;
using UEATSerializer.UE;

namespace UEATSerializer.UEAT
{
    public class UClass : UStruct
    {
        /// <summary>
        /// UClass's EClassFlags value. Automatically serializes ONLY flags that should be loaded (& ~CLASS_ShouldNeverBeLoaded).
        /// </summary>
        public uint ClassFlags { get; set; } = (uint)EClassFlags.CLASS_None;

        public PackageObject ClassWithin { get; set; }
        public string ClassConfigName { get; set; } = string.Empty;
        public List<FImplementedInterface> Interfaces { get; set; } = new List<FImplementedInterface>();
        public PackageObject ClassDefaultObject { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.UnionWith(base.ResolveObjectReferences(objectHierarchy));

            referencedObjects.Add(objectHierarchy.AddUnique(ClassWithin));
            referencedObjects.Add(objectHierarchy.AddUnique(ClassDefaultObject));

            if (ClassWithin != null)
            {
                referencedObjects.UnionWith(ClassWithin.ResolveObjectReferences(objectHierarchy));
            }

            if (ClassDefaultObject != null)
            {
                referencedObjects.UnionWith(ClassDefaultObject.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("ClassFlags");
            // for some reason this should be serialized as a string
            writer.WriteValue((ClassFlags & (uint)~EClassFlags.CLASS_ShouldNeverBeLoaded).ToString());
            writer.WritePropertyName("ClassWithin");
            writer.WriteValue(objectHierarchy.GetObjectIndex(ClassWithin?.Id));

            writer.WritePropertyName("Interfaces");
            writer.WriteStartArray();
            foreach (var implementedInterface in Interfaces)
            {
                if (implementedInterface == null)
                {
                    continue;
                }

                implementedInterface.WriteJson(writer, serializer, objectHierarchy);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("ClassDefaultObject");
            writer.WriteValue(objectHierarchy.GetObjectIndex(ClassDefaultObject?.Id));
        }
    }
}
