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

            UEATSerializer.UEAT.UBlueprint blueprintClass = new UEATSerializer.UEAT.UBlueprint();

            UObjectUtils.PopulateUObjectData(blueprintGenClass, blueprintClass, context);
            UStructUtils.PopulateUStructData(blueprintGenClass, blueprintClass, context);
            UClassUtils.PopulateUClassData(blueprintGenClass, blueprintClass, context);
            UBlueprintUtils.PopulateUBlueprintClassData(blueprintGenClass, blueprintClass, context);

            return blueprintClass;
        }
    }
}
