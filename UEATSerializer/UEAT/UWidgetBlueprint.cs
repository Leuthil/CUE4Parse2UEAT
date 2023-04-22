using Newtonsoft.Json;
using UEATSerializer.Serializer;

namespace UEATSerializer.UEAT
{
    public class UWidgetBlueprint : UBlueprint
    {
        public List<UMovieSceneEventTriggerSection> MovieSceneEventTriggerSection { get; set; } = new List<UMovieSceneEventTriggerSection>();

        protected override void WriteJsonForData(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            base.WriteJsonForData(writer, serializer, objectHierarchy);

            writer.WritePropertyName("MovieSceneEventTriggerSectionFunctions");
            writer.WriteStartObject();
            foreach (var movieSceneEventTriggerSection in MovieSceneEventTriggerSection)
            {
                // write inlined so each section is its own json property
                movieSceneEventTriggerSection.WriteJsonInlined(writer, serializer, objectHierarchy);
            }
            writer.WriteEndObject();
        }
    }


    public class UMovieSceneEventTriggerSection : ISerializableForUEAT
    {
        // EventSection->GetName()
        public string EventSectionName { get; set; }

        // EventSection->EventChannel.GetData().GetValues()
        public List<FMovieSceneEvent> Functions { get; set; } = new List<FMovieSceneEvent>();

        public void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartObject();
            WriteJsonInlined(writer, serializer, objectHierarchy);
            writer.WriteEndObject();
        }

        public void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WritePropertyName(EventSectionName);
            writer.WriteStartArray();
            foreach (var func in Functions)
            {
                func.WriteJson(writer, serializer, objectHierarchy);
            }
            writer.WriteEndArray();
        }
    }

    // EventSection->EventChannel.GetData().GetValues()[i];
    public class FMovieSceneEvent : ISerializableForUEAT
    {
        // EventChannel NumKey Index
        public int KeyIndex { get; set; }

        // MovieSceneEvent.Ptrs.Function->GetName()
        public string FunctionName { get; set; }

        // MovieSceneEvent.Ptrs.BoundObjectProperty.ToString()
        public FFieldPathPropertyValue BoundObjectProperty { get; set; }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WriteStartObject();
            WriteJsonInlined(writer, serializer, objectHierarchy);
            writer.WriteEndObject();
        }

        public void WriteJsonInlined(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WritePropertyName("KeyIndex");
            writer.WriteValue(KeyIndex);
            writer.WritePropertyName("FunctionName");
            writer.WriteValue(FunctionName);

            writer.WritePropertyName("BoundObjectProperty");
            if (BoundObjectProperty == null)
            {
                writer.WriteValue(BoundObjectProperty);
            }
            else
            {
                BoundObjectProperty.WriteJson(writer, serializer, objectHierarchy);
            }
        }
    }
}
