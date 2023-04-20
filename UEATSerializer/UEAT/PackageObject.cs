using UEATSerializer.Serializer;
using UEATSerializer.UE;
using Newtonsoft.Json;

namespace UEATSerializer.UEAT
{
    public abstract class PackageObject : UObject
    {
    }

    public class ImportPackageObject : PackageObject
    {
        public string ClassPackage { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public PackageObject Outer { get; set; }

        [Obsolete("Should not be used for imports")]
        public new Dictionary<string, FPropertyValue> Properties { get; set; } = new Dictionary<string, FPropertyValue>();

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.Add(objectHierarchy.AddUnique(Outer));

            if (Outer != null)
            {
                referencedObjects.UnionWith(Outer.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WritePropertyName("Type");
            writer.WriteValue("Import");
            writer.WritePropertyName("ClassPackage");
            writer.WriteValue(ClassPackage);
            writer.WritePropertyName("ClassName");
            writer.WriteValue(ClassName);
            writer.WritePropertyName("ObjectName");
            writer.WriteValue(ObjectName);

            if (Outer != null)
            {
                writer.WritePropertyName("Outer");
                writer.WriteValue(objectHierarchy.GetObjectIndex(Outer?.Id));
            }
        }
    }

    public class ExportPackageObject : PackageObject
    {
        public PackageObject ObjectClass { get; set; }
        public PackageObject Outer { get; set; }

        /// <summary>
        /// UObject's EObjectFlags value. Automatically serializes ONLY flags that match RF_Load specification.
        /// </summary>
        public int ObjectFlags { get; set; } = (int)EObjectFlags.RF_NoFlags;

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            referencedObjects.Add(objectHierarchy.AddUnique(ObjectClass));
            referencedObjects.Add(objectHierarchy.AddUnique(Outer));

            if (ObjectClass != null)
            {
                referencedObjects.UnionWith(ObjectClass.ResolveObjectReferences(objectHierarchy));
            }

            if (Outer != null)
            {
                referencedObjects.UnionWith(Outer.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WritePropertyName("Type");
            writer.WriteValue("Export");

            // if package object is the asset object for the package, then mark it as such and exit early
            if (objectHierarchy.PackageUObjectAsset.ObjectName.Equals(ObjectName))
            {
                writer.WritePropertyName("ObjectMark");
                writer.WriteValue("$AssetObject$");
                return;
            }

            writer.WritePropertyName("ObjectClass");
            writer.WriteValue(objectHierarchy.GetObjectIndex(ObjectClass?.Id));

            if (Outer != null)
            {
                writer.WritePropertyName("Outer");
                writer.WriteValue(objectHierarchy.GetObjectIndex(Outer?.Id));
            } 

            writer.WritePropertyName("ObjectName");
            writer.WriteValue(ObjectName);
            writer.WritePropertyName("ObjectFlags");
            writer.WriteValue(ObjectFlags & (int)EObjectFlags.RF_Load);

            writer.WritePropertyName("Properties");
            writer.WriteStartObject();

            HashSet<int> referencedObjects = new HashSet<int>();

            foreach (var property in Properties)
            {
                writer.WritePropertyName(property.Key);
                property.Value.WriteJson(writer, serializer, objectHierarchy);

                referencedObjects.UnionWith(property.Value.ResolveObjectReferences(objectHierarchy));
            }

            writer.WritePropertyName("$ReferencedObjects");
            writer.WriteStartArray();

            foreach (int referencedPackageObjectIndex in referencedObjects)
            {
                writer.WriteValue(referencedPackageObjectIndex);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
