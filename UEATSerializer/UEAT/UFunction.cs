using Newtonsoft.Json;
using UEATSerializer.Serializer;

namespace UEATSerializer.UEAT
{
    public class UFunction : UStruct
    {
        public string FunctionFlags { get; set; } = string.Empty;
        public string EventGraphFunction { get; set; } = string.Empty;
        public int EventGraphCallOffset { get; set; }

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WritePropertyName("ObjectClass");
            writer.WriteValue("Function"); // hardcoded value
            writer.WritePropertyName("ObjectName");
            writer.WriteValue(ObjectName);

            base.WriteJsonInlined(writer, serializer, objectHierarchy);

            writer.WritePropertyName("FunctionFlags");
            writer.WriteValue(FunctionFlags);

            if (!string.IsNullOrEmpty(EventGraphFunction))
            {
                writer.WritePropertyName("EventGraphFunction");
                writer.WriteValue(EventGraphFunction);
                writer.WritePropertyName("EventGraphCallOffset");
                writer.WriteValue(EventGraphCallOffset);
            }
        }
    }
}
