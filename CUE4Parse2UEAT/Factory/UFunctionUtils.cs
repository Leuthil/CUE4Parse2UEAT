using CUE4Parse.UE4.Assets;

namespace CUE4Parse2UEAT.Factory
{
    public static class UFunctionUtils
    {
        public static UEATSerializer.UEAT.UFunction? CreateUFunction(CUE4Parse.UE4.Objects.UObject.UFunction func, IoPackage package)
        {
            if (func == null)
            {
                return null;
            }

            var ufunction = new UEATSerializer.UEAT.UFunction();

            UObjectUtils.PopulateUObjectData(func, ufunction, package);
            UStructUtils.PopulateUStructData(func, ufunction, package);

            ufunction.FunctionFlags = func.FunctionFlags.ToString();
            ufunction.EventGraphFunction = func.EventGraphFunction.Name;
            ufunction.EventGraphCallOffset = func.EventGraphCallOffset;

            return ufunction;
        }
    }
}
