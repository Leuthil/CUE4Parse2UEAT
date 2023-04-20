using UEATSerializer.Serializer;
using Newtonsoft.Json;

namespace UEATSerializer.UEAT
{
    public class UBlueprint : UClass
    {
        // (Composed of variables from: SimpleConstructionScript.GetAllNodes.GetVariableName, Timelines.GetVariableName,
        //                              Timelines.GetDirectionPropertyName, Timelines.FlaotTracks, Timelines.VectorTracks,
        //                              Timelines.LinearColorTracks)
        public List<string> GeneratedVariableNames { get; set; } = new List<string>();

        protected override void WriteJsonForData(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonForData(writer, serializer, objectHierarchy);

            writer.WritePropertyName("GeneratedVariableNames");
            writer.WriteStartArray();

            foreach (var generatedVariableName in GeneratedVariableNames)
            {
                writer.WriteValue(generatedVariableName);
            }

            writer.WriteEndArray();
        }
    }
}
