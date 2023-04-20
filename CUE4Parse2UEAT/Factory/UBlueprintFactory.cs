using CUE4Parse.UE4.Assets;

namespace CUE4Parse2UEAT.Factory
{
    public class UBlueprintFactory : IUObjectFactory
    {
        public int Priority => 0;

        public bool CanHandle(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, IoPackage package)
        {
            return assetObject is CUE4Parse.UE4.Objects.Engine.UBlueprintGeneratedClass;
        }

        public UEATSerializer.UEAT.UObject? CreateUObject(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, IoPackage package)
        {
            var blueprintGenClass = assetObject as CUE4Parse.UE4.Objects.Engine.UBlueprintGeneratedClass;

            if (blueprintGenClass == null)
            {
                return null;
            }

            UEATSerializer.UEAT.UBlueprint blueprintClass = new UEATSerializer.UEAT.UBlueprint();

            UObjectUtils.PopulateUObjectData(blueprintGenClass, blueprintClass, package);
            UStructUtils.PopulateUStructData(blueprintGenClass, blueprintClass, package);
            UClassUtils.PopulateUClassData(blueprintGenClass, blueprintClass, package);
            UBlueprintUtils.PopulateUBlueprintClassData(blueprintGenClass, blueprintClass, package);

            return blueprintClass;
        }
    }
}
