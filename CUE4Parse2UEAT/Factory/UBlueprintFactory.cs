using CUE4Parse2UEAT.Generation;

namespace CUE4Parse2UEAT.Factory
{
    public class UBlueprintFactory : IUObjectFactory
    {
        public int Priority => 0;

        public bool CanHandle(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context)
        {
            return assetObject is CUE4Parse.UE4.Objects.Engine.UBlueprintGeneratedClass;
        }

        public UEATSerializer.UEAT.UObject? CreateUObject(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context)
        {
            var blueprintGenClass = assetObject as CUE4Parse.UE4.Objects.Engine.UBlueprintGeneratedClass;

            if (blueprintGenClass == null)
            {
                return null;
            }

            UEATSerializer.UEAT.UBlueprint blueprint = new UEATSerializer.UEAT.UBlueprint();

            UObjectUtils.PopulateUObjectIdentification(blueprintGenClass, blueprint, context);
            UObjectUtils.PopulateUObjectProperties(blueprintGenClass, blueprint, context);
            UStructUtils.PopulateUStructData(blueprintGenClass, blueprint, context);
            UClassUtils.PopulateUClassData(blueprintGenClass, blueprint, context);
            UBlueprintUtils.PopulateUBlueprintData(blueprintGenClass, blueprint, context);

            return blueprint;
        }
    }
}
