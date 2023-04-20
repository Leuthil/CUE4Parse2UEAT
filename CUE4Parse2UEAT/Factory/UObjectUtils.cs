using CUE4Parse.UE4.Assets;

namespace CUE4Parse2UEAT.Factory
{
    public static class UObjectUtils
    {
        public readonly static List<IUObjectFactory> UObjectFactories = new List<IUObjectFactory>()
        {
            new UBlueprintFactory(),
        };

        public static UEATSerializer.UEAT.UObject? CreateUObject(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, IoPackage package)
        {
            var factory = UObjectFactories.FindAll(f => f.CanHandle(assetObject, package)).OrderByDescending(f => f.Priority).FirstOrDefault();

            if (factory == null)
            {
                return null;
            }

            return factory.CreateUObject(assetObject, package);
        }

        public static void PopulateUObjectData(CUE4Parse.UE4.Assets.Exports.UObject cue4ParseUObject,
            UEATSerializer.UEAT.UObject ueatUObject, IoPackage package)
        {
            ueatUObject.PackageName = cue4ParseUObject.Owner?.Name;
            ueatUObject.ObjectName = cue4ParseUObject.Name;

            foreach (var property in cue4ParseUObject.Properties)
            {
                var entryName = property.Name.Text;
                var entryValue = FPropertyValueUtils.CreateFPropertyValue(property.Tag, package);

                if (entryValue == null)
                {
                    continue;
                }

                ueatUObject.Properties.Add(entryName, entryValue);
            }
        }
    }
}
