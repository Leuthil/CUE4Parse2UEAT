using Newtonsoft.Json;
using UEATSerializer.Serializer;

namespace UEATSerializer.UEAT
{
    public class UBlueprint : UClass
    {
        // (Composed of variables from: SimpleConstructionScript.GetAllNodes.GetVariableName, Timelines.GetVariableName,
        //                              Timelines.GetDirectionPropertyName, Timelines.FlaotTracks, Timelines.VectorTracks,
        //                              Timelines.LinearColorTracks)
        public HashSet<string> GeneratedVariableNames { get; set; } = new HashSet<string>();

        public override void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            // workaround to always serialize a SimpleConstructionScript property even if it doesn't exist
            if (!Properties.Any(p => p.Key.Equals("SimpleConstructionScript")))
            {
                Properties.Add(KeyValuePair.Create("SimpleConstructionScript", (FPropertyValue)new FObjectPropertyBaseValue()));
            }

            base.WriteJsonInlined(writer, serializer, objectHierarchy);
        }

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
