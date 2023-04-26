using CUE4Parse2UEAT.Factory;

namespace CUE4Parse2UEAT.Generation
{
    public static class UFunctionUtils
    {
        public static UEATSerializer.UEAT.UFunction? CreateUFunction(CUE4Parse.UE4.Objects.UObject.UFunction func,
            IPackageObjectFactory packageObjectFactory)
        {
            if (func == null)
            {
                return null;
            }

            var ufunction = new UEATSerializer.UEAT.UFunction();

            UObjectUtils.PopulateUObjectIdentification(func, ufunction);
            UObjectUtils.PopulateUObjectProperties(func, ufunction, packageObjectFactory);
            UStructUtils.PopulateUStructData(func, ufunction, packageObjectFactory);

            ufunction.FunctionFlags = func.FunctionFlags.ToString();
            ufunction.EventGraphFunction = func.EventGraphFunction.Name;
            ufunction.EventGraphCallOffset = func.EventGraphCallOffset;

            return ufunction;
        }
    }
}
