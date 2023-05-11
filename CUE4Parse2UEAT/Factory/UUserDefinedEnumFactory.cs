using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse2UEAT.Generation;
using UEATSerializer.UEAT;

namespace CUE4Parse2UEAT.Factory
{
    public class UUserDefinedEnumFactory : IUObjectFactory
    {
        public int Priority => 0;

        public bool CanHandle(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context)
        {
            return assetObject is CUE4Parse.UE4.Objects.Engine.UUserDefinedEnum;
        }

        public UEATSerializer.UEAT.UObject? CreateUObject(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context)
        {
            if (assetObject is not CUE4Parse.UE4.Objects.Engine.UUserDefinedEnum cue4parseUserDefinedEnum)
            {
                return null;
            }

            UEATSerializer.UEAT.UUserDefinedEnum userDefinedEnum = new UEATSerializer.UEAT.UUserDefinedEnum();

            UObjectUtils.PopulateUObjectIdentification(cue4parseUserDefinedEnum, userDefinedEnum);
            UObjectUtils.PopulateUObjectProperties(cue4parseUserDefinedEnum, userDefinedEnum, context.PackageObjectFactory);
            UEnumUtils.PopulateUEnumData(cue4parseUserDefinedEnum, userDefinedEnum, context.PackageObjectFactory);

            // CUE4Parse does not have this data
            //userDefinedEnum.DisplayNameMap
            var displayNameMap = UObjectUtils.GetPropertyValue<MapProperty>(cue4parseUserDefinedEnum, "DisplayNameMap")?.Value?.Properties;
            foreach (var displayNameMapEntry in displayNameMap)
            {
                var key = FPropertyValueUtils.CreateFPropertyValue(displayNameMapEntry.Key, context.PackageObjectFactory) as FNamePropertyValue;
                var value = FPropertyValueUtils.CreateFPropertyValue(displayNameMapEntry.Value, context.PackageObjectFactory) as FTextPropertyValue;

                if (key == null || value == null)
                {
                    continue;
                }

                userDefinedEnum.DisplayNameMap.Add(key.Name, value.Value);
            }

            return userDefinedEnum;
        }
    }
}
