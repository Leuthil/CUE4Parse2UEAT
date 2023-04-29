using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse2UEAT.Factory;

namespace CUE4Parse2UEAT.Generation
{
    public static class UObjectUtils
    {
        private readonly static List<IUObjectFactory> UObjectFactories = new List<IUObjectFactory>()
        {
            new UBlueprintFactory(),
            new UWidgetBlueprintFactory(),
        };

        public static UEATSerializer.UEAT.UObject? CreateUObject(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context)
        {
            var factory = UObjectFactories.FindAll(f => f.CanHandle(assetObject, context)).OrderByDescending(f => f.Priority).FirstOrDefault();

            if (factory == null)
            {
                return null;
            }

            return factory.CreateUObject(assetObject, context);
        }

        public static void PopulateUObjectIdentification(CUE4Parse.UE4.Assets.Exports.UObject cue4ParseUObject,
            UEATSerializer.UEAT.UObject ueatUObject)
        {
            ueatUObject.PackageName = cue4ParseUObject.Owner?.Name;
            ueatUObject.ObjectName = cue4ParseUObject.Name;
            ueatUObject.OuterName = cue4ParseUObject.Outer?.Name;
        }

        public static void PopulateUObjectProperties(CUE4Parse.UE4.Assets.Exports.UObject cue4ParseUObject,
            UEATSerializer.UEAT.UObject ueatUObject, IPackageObjectFactory packageObjectFactory)
        {
            foreach (var property in cue4ParseUObject.Properties)
            {
                var entryName = property.Name.Text;
                var entryValue = FPropertyValueUtils.CreateFPropertyValue(property.Tag, packageObjectFactory);

                if (entryValue == null)
                {
                    continue;
                }

                ueatUObject.Properties.Add(KeyValuePair.Create(entryName, entryValue));
            }
        }

        public static T? GetPropertyValue<T>(IPropertyHolder? propertyHolder, string propertyName) where T : FPropertyTagType
        {
            if (propertyHolder == null)
            {
                return null;
            }

            return propertyHolder?.Properties?.Find(p => propertyName.Equals(p?.Name.Text))?.Tag as T;
        }
    }
}
