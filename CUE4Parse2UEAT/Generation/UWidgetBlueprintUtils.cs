using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;

namespace CUE4Parse2UEAT.Generation
{
    public static class UWidgetBlueprintUtils
    {
        public static UObject? FindRootWidget(UObject? widgetBlueprintGenClass)
        {
            if (widgetBlueprintGenClass == null)
            {
                return null;
            }

            var widgetTreePropertyValue = UObjectUtils.GetPropertyValue<ObjectProperty>(widgetBlueprintGenClass, "WidgetTree")?.Value?.ResolvedObject;
            if (widgetTreePropertyValue == null)
            {
                return null;
            }

            if (!widgetTreePropertyValue.TryLoad(out var widgetTree))
            {
                return null;
            }

            var resolvedRootWidget = UObjectUtils.GetPropertyValue<ObjectProperty>(widgetTree, "RootWidget")?.Value?.ResolvedObject;
            if (resolvedRootWidget == null)
            {
                return null;
            }

            if (!resolvedRootWidget.TryLoad(out var rootWidget))
            {
                return null;
            }

            return rootWidget;
        }

        public static void ForEachWidget(UObject? widget, Action<UObject> action)
        {
            if (widget == null)
            {
                return;
            }

            action.Invoke(widget);

            var widgetSlotsPropertyValue = UObjectUtils.GetPropertyValue<ArrayProperty>(widget, "Slots");
            if (widgetSlotsPropertyValue?.Value?.Properties == null)
            {
                return;
            }

            foreach (var slotWidgetPropertyValue in widgetSlotsPropertyValue.Value.Properties.Cast<ObjectProperty>())
            {
                if (slotWidgetPropertyValue?.Value == null)
                {
                    continue;
                }

                if (!slotWidgetPropertyValue.Value.TryLoad(out var slotWidget))
                {
                    continue;
                }

                var contentWidgetPropertyValue = UObjectUtils.GetPropertyValue<ObjectProperty>(widget, "Content")?.Value?.ResolvedObject;
                if (contentWidgetPropertyValue == null)
                {
                    continue;
                }

                if (!contentWidgetPropertyValue.TryLoad(out var contentWidget))
                {
                    continue;
                }

                ForEachWidget(contentWidget, action);
            }
        }

        public static void ForEachAnimation(UObject? widgetBlueprintGenClass, Action<UObject> action)
        {
            var animationsPropertyValue = UObjectUtils.GetPropertyValue<ArrayProperty>(widgetBlueprintGenClass, "Animations");
            if (animationsPropertyValue?.Value?.Properties == null)
            {
                return;
            }

            foreach (var animationPropertyValue in animationsPropertyValue.Value.Properties.Cast<ObjectProperty>())
            {
                if (animationPropertyValue?.Value == null)
                {
                    continue;
                }

                if (!animationPropertyValue.Value.TryLoad(out var animation))
                {
                    continue;
                }

                action.Invoke(animation);
            }
        }
    }
}
