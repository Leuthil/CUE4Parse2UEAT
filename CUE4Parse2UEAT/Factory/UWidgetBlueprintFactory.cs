using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse2UEAT.Generation;

namespace CUE4Parse2UEAT.Factory
{
    public class UWidgetBlueprintFactory : IUObjectFactory
    {
        public int Priority => 1;

        public bool CanHandle(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context)
        {
            return assetObject is CUE4Parse.UE4.Objects.Engine.UWidgetBlueprintGeneratedClass;
        }

        public UEATSerializer.UEAT.UObject? CreateUObject(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context)
        {
            var widgetBlueprintGenClass = assetObject as CUE4Parse.UE4.Objects.Engine.UWidgetBlueprintGeneratedClass;

            if (widgetBlueprintGenClass == null)
            {
                return null;
            }

            UEATSerializer.UEAT.UWidgetBlueprint widgetBlueprint = new UEATSerializer.UEAT.UWidgetBlueprint();

            UObjectUtils.PopulateUObjectIdentification(widgetBlueprintGenClass, widgetBlueprint);
            UObjectUtils.PopulateUObjectProperties(widgetBlueprintGenClass, widgetBlueprint, context.PackageObjectFactory);
            UStructUtils.PopulateUStructData(widgetBlueprintGenClass, widgetBlueprint, context.PackageObjectFactory);
            UClassUtils.PopulateUClassData(widgetBlueprintGenClass, widgetBlueprint, context.PackageObjectFactory);
            UBlueprintUtils.PopulateUBlueprintData(widgetBlueprintGenClass, widgetBlueprint);

            var rootWidget = UWidgetBlueprintUtils.FindRootWidget(widgetBlueprintGenClass);
            UWidgetBlueprintUtils.ForEachWidget(rootWidget, (widget) =>
            {
                var isVariablePropertyValue = UObjectUtils.GetPropertyValue<BoolProperty>(widget, "bIsVariable");
                if (isVariablePropertyValue != null)
                {
                    if (isVariablePropertyValue.Value)
                    {
                        widgetBlueprint.GeneratedVariableNames.Add(widget.Name);
                    }
                }
            });

            UWidgetBlueprintUtils.ForEachAnimation(widgetBlueprintGenClass, (animation) =>
            {
                var originalAnimationName = animation.Name;

                if (originalAnimationName.EndsWith("_INST"))
                {
                    originalAnimationName = originalAnimationName.Substring(0, originalAnimationName.Length - "_INST".Length);
                }

                widgetBlueprint.GeneratedVariableNames.Add(originalAnimationName);

                var movieScene = UObjectUtils.GetPropertyValue<ObjectProperty>(animation, "MovieScene")?.Value?.ResolvedObject?.Load();
                var objectBindingsPropertyValue = UObjectUtils.GetPropertyValue<ArrayProperty>(movieScene, "ObjectBindings");

                if (objectBindingsPropertyValue?.Value?.Properties != null)
                {
                    foreach (var objectBindingPropertyValue in objectBindingsPropertyValue.Value.Properties.Cast<StructProperty>())
                    {
                        if (objectBindingPropertyValue?.Value?.StructType is IPropertyHolder objectBinding)
                        {
                            var objectName = UObjectUtils.GetPropertyValue<StrProperty>(objectBinding, "BindingName")?.Value;

                            if (objectName != null)
                            {
                                widgetBlueprint.GeneratedVariableNames.Add(objectName);
                            }
                        }
                    }
                }
            });

            /*
             * BoundObjectProperty is always empty so this actually just causes problems
            foreach (var export in context.Package.GetExports().Where(e => "MovieSceneEventTriggerSection".Equals(e?.Class?.Name)))
            {
                if (UObjectUtils.GetPropertyValue<StructProperty>(export, "EventChannel")?.Value?.StructType is not IPropertyHolder eventChannel)
                {
                    continue;
                }

                var eventSection = new UMovieSceneEventTriggerSection();
                eventSection.EventSectionName = export.Name;

                widgetBlueprint.MovieSceneEventTriggerSection.Add(eventSection);

                var keyValuesPropertyValue = UObjectUtils.GetPropertyValue<ArrayProperty>(eventChannel, "KeyValues");
                if (keyValuesPropertyValue?.Value?.Properties == null)
                {
                    continue;
                }

                var keyValues = keyValuesPropertyValue.Value.Properties;
                for (int i = 0; i < keyValues.Count; i++)
                {
                    var ptrs = UObjectUtils.GetPropertyValue<StructProperty>((keyValues[i] as StructProperty)?.Value?.StructType as FStructFallback, "Ptrs")?.Value?.StructType as FStructFallback;
                    if (ptrs == null)
                    {
                        continue;
                    }

                    var movieSceneEvent = new FMovieSceneEvent();
                    movieSceneEvent.KeyIndex = i;
                    movieSceneEvent.FunctionName = UObjectUtils.GetPropertyValue<ObjectProperty>(ptrs, "Function")?.Value?.Name;
                    // this seems to always be null / empty
                    var boundObjectProperty = UObjectUtils.GetPropertyValue<FieldPathProperty>(ptrs, "BoundObjectProperty");
                    movieSceneEvent.BoundObjectProperty = (FFieldPathPropertyValue?)FPropertyValueUtils.CreateFPropertyValue(boundObjectProperty, context.PackageObjectFactory);

                    eventSection.Functions.Add(movieSceneEvent);
                }
            }
            */

            // add named slots as variable names?
            // add "bindings" as variable names? Is it DynamicBindingObjects property?

            return widgetBlueprint;
        }
    }
}
