using CUE4Parse.UE4.Assets;

namespace CUE4Parse2UEAT.Factories.CUE4P
{
    public static class UObjectUtils
    {
        public static UEATSerializer.UEAT.UObject? CreateUObject(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, IoPackage package)
        {
            if (assetObject is CUE4Parse.UE4.Objects.Engine.UBlueprintGeneratedClass bpObject)
            {
                UEATSerializer.UEAT.UBlueprintClass blueprintClass = new UEATSerializer.UEAT.UBlueprintClass();

                blueprintClass.PackageName = bpObject.Owner?.Name;
                blueprintClass.ObjectName = bpObject.Name;

                foreach (var property in bpObject.Properties)
                {
                    var entryName = property.Name.Text;
                    var entryValue = FPropertyValueUtils.CreateFPropertyValue(property.Tag, package);

                    if (entryValue == null)
                    {
                        continue;
                    }

                    blueprintClass.Properties.Add(entryName, entryValue);
                }

                blueprintClass.SuperStruct = PackageObjectUtils.CreatePackageObject(bpObject.SuperStruct.ResolvedObject, package);

                foreach (var child in bpObject.Children)
                {
                    if (!child.TryLoad(out CUE4Parse.UE4.Assets.Exports.UObject? childExport))
                    {
                        continue;
                    }

                    if (childExport is not CUE4Parse.UE4.Objects.UObject.UFunction func)
                    {
                        continue;
                    }

                    var ufunction = UFunctionUtils.CreateUFunction(func, package);

                    if (ufunction == null)
                    {
                        continue;
                    }

                    blueprintClass.Children.Add(ufunction);
                }

                foreach (var childProperty in bpObject.ChildProperties)
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

                    blueprintClass.ChildProperties.Add(fprop.Name.Text, fproperty);
                }

                // (requires bytecode disassembly, not sure if available) (BytecodeDisassembler.SerializeFunction(struct))
                //blueprintClass.Script = new List<object>();

                blueprintClass.ClassFlags = bpObject.ClassFlags;
                blueprintClass.ClassWithin = PackageObjectUtils.CreatePackageObject(bpObject.ClassWithin.ResolvedObject, package);
                blueprintClass.ClassConfigName = bpObject.ClassConfigName.Text;

                foreach (var implInterface in bpObject.Interfaces)
                {
                    if (implInterface == null)
                    {
                        continue;
                    }

                    var implementedInterface = new UEATSerializer.UEAT.FImplementedInterface();

                    implementedInterface.Class = PackageObjectUtils.CreatePackageObject(implInterface.Class.ResolvedObject, package);
                    implementedInterface.PointerOffset = implInterface.PointerOffset;
                    implementedInterface.bImplementedByK2 = implInterface.bImplementedByK2;

                    blueprintClass.Interfaces.Add(implementedInterface);
                }

                blueprintClass.ClassDefaultObject = PackageObjectUtils.CreatePackageObject(bpObject.ClassDefaultObject.ResolvedObject, package);

                // SCS and Timeline data doesn't seem available in CUE4Parse

                // GeneratedVariableNames (string[])
                // (Composed of variables from: SimpleConstructionScript.GetAllNodes.GetVariableName, Timelines.GetVariableName,
                //                              Timelines.GetDirectionPropertyName, Timelines.FlaotTracks, Timelines.VectorTracks,
                //                              Timelines.LinearColorTracks)
                //blueprinceClass.GeneratedVariableNames = new List<string>();

                return blueprintClass;
            }

            return null;
        }
    }
}
