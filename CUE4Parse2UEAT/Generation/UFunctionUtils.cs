namespace CUE4Parse2UEAT.Generation
{
    public static class UFunctionUtils
    {
        public static UEATSerializer.UEAT.UFunction? CreateUFunction(CUE4Parse.UE4.Objects.UObject.UFunction func, GenerationContext context)
        {
            if (func == null)
            {
                return null;
            }

            var ufunction = new UEATSerializer.UEAT.UFunction();

            UObjectUtils.PopulateUObjectIdentification(func, ufunction, context);
            UObjectUtils.PopulateUObjectProperties(func, ufunction, context);
            UStructUtils.PopulateUStructData(func, ufunction, context);

            ufunction.FunctionFlags = func.FunctionFlags.ToString();
            ufunction.EventGraphFunction = func.EventGraphFunction.Name;
            ufunction.EventGraphCallOffset = func.EventGraphCallOffset;

            return ufunction;
        }
    }
}
