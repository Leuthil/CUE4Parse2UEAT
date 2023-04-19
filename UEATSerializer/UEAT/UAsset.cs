using UEATSerializer.Serializer;
using Newtonsoft.Json;

namespace UEATSerializer.UEAT
{
    [JsonConverter(typeof(UAssetConverter))]
    public class UAsset : UObject
    {
        public string ClassName { get; set; } = string.Empty;

        public UObject UObjectAsset { get; set; }

        public IEnumerable<PackageObject> ImportPackageObjects { get; set; }
        public IEnumerable<PackageObject> ExportPackageObjects { get; set; }

        public override int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy)
        {
            HashSet<int> referencedObjects = new HashSet<int>();

            if (UObjectAsset != null)
            {
                referencedObjects.UnionWith(UObjectAsset.ResolveObjectReferences(objectHierarchy));
            }

            foreach (var importPackageObject in ImportPackageObjects)
            {
                referencedObjects.Add(objectHierarchy.AddUnique(importPackageObject));

                if (importPackageObject != null)
                {
                    referencedObjects.UnionWith(importPackageObject.ResolveObjectReferences(objectHierarchy));
                }
            }

            foreach (var exportPackageObject in ExportPackageObjects)
            {
                referencedObjects.Add(objectHierarchy.AddUnique(exportPackageObject));

                if (exportPackageObject != null)
                {
                    referencedObjects.UnionWith(exportPackageObject.ResolveObjectReferences(objectHierarchy));
                }
            }

            return referencedObjects.ToArray();
        }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WritePropertyName("AssetClass");
            writer.WriteValue(ClassName);
            writer.WritePropertyName("AssetPackage");
            writer.WriteValue(PackageName);
            writer.WritePropertyName("AssetName");
            writer.WriteValue(ObjectName);
            writer.WritePropertyName("AssetSerializedData");
            UObjectAsset.WriteJson(writer, serializer, objectHierarchy);

            writer.WritePropertyName("ObjectHierarchy");
            writer.WriteStartArray();

            foreach (var packageObjectIndexPair in objectHierarchy.PackageObjects)
            {
                var packageObject = packageObjectIndexPair.Key;
                var index = packageObjectIndexPair.Value;

                writer.WriteStartObject();

                writer.WritePropertyName("ObjectIndex");
                writer.WriteValue(index);
                packageObject.WriteJsonInlined(writer, serializer, objectHierarchy);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    internal class UAssetConverter : JsonConverter<UAsset>
    {
        public override void WriteJson(JsonWriter writer, UAsset? value, JsonSerializer serializer)
        {
            PackageObjectHierarchy objectHierarchy = new PackageObjectHierarchy(value.UObjectAsset);

            value.ResolveObjectReferences(objectHierarchy);
            value.WriteJson(writer, serializer, objectHierarchy);
        }

        public override UAsset? ReadJson(JsonReader reader, Type objectType, UAsset? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
