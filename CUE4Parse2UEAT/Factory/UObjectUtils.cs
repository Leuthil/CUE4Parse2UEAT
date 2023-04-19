using CUE4Parse.UE4.Assets;

namespace CUE4Parse2UEAT.Factory
{
    public static class UObjectUtils
    {
        public readonly static List<IUObjectFactory> UObjectFactories = new List<IUObjectFactory>()
        {
            new UBlueprintClassFactory(),
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
    }
}
