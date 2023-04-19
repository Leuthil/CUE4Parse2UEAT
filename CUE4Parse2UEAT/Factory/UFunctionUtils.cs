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

            // function-specific data
            ufunction.FunctionFlags = func.FunctionFlags.ToString();
            ufunction.EventGraphFunction = func.EventGraphFunction.Name;
            ufunction.EventGraphCallOffset = func.EventGraphCallOffset;

            // uobject and ustruct-specific data
            ufunction.PackageName = func.Owner?.Name;
            ufunction.ObjectName = func.Name;

            foreach (var property in func.Properties)
            {
                var entryName = property.Name.Text;
                var entryValue = FPropertyValueUtils.CreateFPropertyValue(property.Tag, package);

                if (entryValue == null)
                {
                    continue;
                }

                ufunction.Properties.Add(entryName, entryValue);
            }

            ufunction.SuperStruct = PackageObjectUtils.CreatePackageObject(func.SuperStruct.ResolvedObject, package);

            foreach (var child in func.Children)
            {
                if (!child.TryLoad(out CUE4Parse.UE4.Assets.Exports.UObject? childExport))
                {
                    continue;
                }

                if (childExport is not CUE4Parse.UE4.Objects.UObject.UFunction childFunc)
                {
                    continue;
                }

                var childUfunction = CreateUFunction(childFunc, package);

                if (childUfunction == null)
                {
                    continue;
                }

                ufunction.Children.Add(childUfunction);
            }

            foreach (var childProperty in func.ChildProperties)
            {
                if (childProperty is not CUE4Parse.UE4.Objects.UObject.FProperty fprop)
                {
                    continue;
                }

                var fproperty = FPropertyUtils.CreateFProperty(fprop, package);

                if (fproperty == null)
                {
                    continue;
                }

                ufunction.ChildProperties.Add(fprop.Name.Text, fproperty);
            }

            // (requires bytecode disassembly, not sure if available) (BytecodeDisassembler.SerializeFunction(struct))
            //public List<object> Script { get; set; } = new List<object>();

            return ufunction;
        }
    }
}
