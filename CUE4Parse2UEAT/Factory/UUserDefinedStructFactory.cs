using CUE4Parse2UEAT.Generation;
using UEATSerializer.UEAT;

namespace CUE4Parse2UEAT.Factory
{
    internal class UUserDefinedStructFactory : IUObjectFactory
    {
        public int Priority => 0;

        public bool CanHandle(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context)
        {
            return assetObject is CUE4Parse.UE4.Objects.Engine.UUserDefinedStruct;
        }

        public UEATSerializer.UEAT.UObject? CreateUObject(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context)
        {
            var cue4ParseUserDefinedStruct = assetObject as CUE4Parse.UE4.Objects.Engine.UUserDefinedStruct;

            if (cue4ParseUserDefinedStruct == null)
            {
                return null;
            }

            UEATSerializer.UEAT.UUserDefinedStruct ueatUserDefinedStruct = new UEATSerializer.UEAT.UUserDefinedStruct();

            UObjectUtils.PopulateUObjectIdentification(cue4ParseUserDefinedStruct, ueatUserDefinedStruct);
            UObjectUtils.PopulateUObjectProperties(cue4ParseUserDefinedStruct, ueatUserDefinedStruct, context.PackageObjectFactory);
            UStructUtils.PopulateUStructData(cue4ParseUserDefinedStruct, ueatUserDefinedStruct, context.PackageObjectFactory);

            ueatUserDefinedStruct.StructFlags = cue4ParseUserDefinedStruct.StructFlags;

            var guidProp = UObjectUtils.GetPropertyValue<CUE4Parse.UE4.Assets.Objects.StructProperty>(cue4ParseUserDefinedStruct, "Guid");
            ueatUserDefinedStruct.Guid = FPropertyValueUtils.CreateFPropertyValue(guidProp, context.PackageObjectFactory) as FGuidPropertyValue ?? new FGuidPropertyValue(0, 0, 0, 0);

            if (cue4ParseUserDefinedStruct.DefaultProperties != null)
            {
                foreach (var property in cue4ParseUserDefinedStruct.DefaultProperties)
                {
                    var entryName = property.Name.Text;
                    var entryValue = FPropertyValueUtils.CreateFPropertyValue(property.Tag, context.PackageObjectFactory);

                    if (entryValue == null)
                    {
                        continue;
                    }

                    ueatUserDefinedStruct.StructDefaultInstanceProperties.Add(KeyValuePair.Create(entryName, entryValue));
                }
            }

            return ueatUserDefinedStruct;
        }
    }
}
