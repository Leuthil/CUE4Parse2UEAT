using Newtonsoft.Json;
using UEATSerializer.Serializer;

namespace UEATSerializer.UEAT
{
    public abstract class FStructPropertyValue : FPropertyValue
    {
    }

    public class FDateTimeStructPropertyValue : FStructPropertyValue
    {
        /// <summary>
        /// Number of ticks since midnight, January 1, 0001.
        /// </summary>
        public ulong Ticks { get; set; }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartObject();
            // should be serialized as string
            writer.WriteValue(Ticks.ToString());
            writer.WriteEndObject();
        }
    }

    public class FTimespanStructPropertyValue : FStructPropertyValue
    {
        /// <summary>
        /// Number of ticks since midnight, January 1, 0001.
        /// </summary>
        public ulong Ticks { get; set; }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartObject();
            // should be serialized as string
            writer.WriteValue(Ticks.ToString());
            writer.WriteEndObject();
        }
    }

    public class FFallbackStructPropertyValue : FStructPropertyValue
    {
        public List<KeyValuePair<string, FPropertyValue>> Properties { get; set; } = new List<KeyValuePair<string, FPropertyValue>>();

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            foreach (var property in Properties)
            {
                referencedObjects.UnionWith(property.Value.ResolveObjectReferences(objectHierarchy));
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartObject();

            foreach (var property in Properties)
            {
                writer.WritePropertyName(property.Key);
                property.Value.WriteJson(writer, serializer, objectHierarchy);
            }

            writer.WriteEndObject();
        }
    }
}
