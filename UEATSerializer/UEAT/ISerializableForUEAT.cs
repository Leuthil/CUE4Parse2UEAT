using Newtonsoft.Json;
using UEATSerializer.Serializer;

namespace UEATSerializer.UEAT
{
    public interface ISerializableForUEAT
    {
        void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy);
        void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy);
    }
}
