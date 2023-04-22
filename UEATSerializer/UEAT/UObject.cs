using Newtonsoft.Json;
using UEATSerializer.Serializer;

namespace UEATSerializer.UEAT
{
    public abstract class UObject : IEquatable<UObject>, IUObjectPointer, ISerializableForUEAT
    {
        public UObjectIdentifier Id => new UObjectIdentifier(PackageName, ObjectName);
        public string PackageName { get; set; }
        public string ObjectName { get; set; }
        public List<KeyValuePair<string, FPropertyValue>> Properties { get; set; } = new List<KeyValuePair<string, FPropertyValue>>();

        public virtual int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedPackageObjects = new HashSet<int>();

            foreach (var property in Properties)
            {
                referencedPackageObjects.UnionWith(property.Value.ResolveObjectReferences(objectHierarchy));
            }

            return referencedPackageObjects.ToArray();
        }

        public virtual void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartObject();
            WriteJsonInlined(writer, serializer, objectHierarchy);
            writer.WriteEndObject();
        }

        public virtual void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            writer.WritePropertyName("SkipDependencies");
            writer.WriteValue(false);

            writer.WritePropertyName("AssetObjectData");
            writer.WriteStartObject();
            foreach (var property in Properties)
            {
                if (property.Value == null)
                {
                    continue;
                }

                writer.WritePropertyName(property.Key);
                property.Value.WriteJson(writer, serializer, objectHierarchy);

                referencedObjects.UnionWith(property.Value.ResolveObjectReferences(objectHierarchy));
            }

            writer.WritePropertyName("$ReferencedObjects");
            writer.WriteStartArray();

            // remove -1 entry if exists
            referencedObjects.Remove(-1);

            foreach (int referencedPackageObjectIndex in referencedObjects)
            {
                writer.WriteValue(referencedPackageObjectIndex);
            }

            writer.WriteEndArray();
            writer.WriteEndObject();

            WriteJsonForData(writer, serializer, objectHierarchy);
        }

        /// <summary>
        /// Writes asset data not included in Properties.
        /// </summary>
        protected virtual void WriteJsonForData(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
        }

        public bool Equals(UObject? other)
        {
            return Id.Equals(other?.Id);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not UObject)
            {
                return false;
            }

            return Id.Equals(((UObject)obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool Equals(UObject x, UObject y)
        {
            if (x == null)
            {
                return y == null;
            }

            return x.Id.Equals(y.Id);
        }
    }
}
