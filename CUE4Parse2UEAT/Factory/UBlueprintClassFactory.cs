using CUE4Parse.UE4.Assets;

namespace CUE4Parse2UEAT.Factory
{
    public class UBlueprintClassFactory : IUObjectFactory
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

            UEATSerializer.UEAT.UBlueprintClass blueprintClass = new UEATSerializer.UEAT.UBlueprintClass();

            blueprintClass.PackageName = blueprintGenClass.Owner?.Name;
            blueprintClass.ObjectName = blueprintGenClass.Name;

            foreach (var property in blueprintGenClass.Properties)
            {
                var entryName = property.Name.Text;
                var entryValue = FPropertyValueUtils.CreateFPropertyValue(property.Tag, package);

                if (entryValue == null)
                {
                    continue;
                }

                blueprintClass.Properties.Add(entryName, entryValue);
            }

            blueprintClass.SuperStruct = PackageObjectUtils.CreatePackageObject(blueprintGenClass.SuperStruct.ResolvedObject, package);

            foreach (var child in blueprintGenClass.Children)
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

            foreach (var childProperty in blueprintGenClass.ChildProperties)
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

            blueprintClass.ClassFlags = blueprintGenClass.ClassFlags;
            blueprintClass.ClassWithin = PackageObjectUtils.CreatePackageObject(blueprintGenClass.ClassWithin.ResolvedObject, package);
            blueprintClass.ClassConfigName = blueprintGenClass.ClassConfigName.Text;

            foreach (var implInterface in blueprintGenClass.Interfaces)
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

            blueprintClass.ClassDefaultObject = PackageObjectUtils.CreatePackageObject(blueprintGenClass.ClassDefaultObject.ResolvedObject, package);

            // SCS and Timeline data doesn't seem available in CUE4Parse
            // Actually I see SCS_Node references within the export objects so maybe it is available

            // GeneratedVariableNames (string[])
            // (Composed of variables from: SimpleConstructionScript.GetAllNodes.GetVariableName, Timelines.GetVariableName,
            //                              Timelines.GetDirectionPropertyName, Timelines.FlaotTracks, Timelines.VectorTracks,
            //                              Timelines.LinearColorTracks)
            //blueprinceClass.GeneratedVariableNames = new List<string>();

            return blueprintClass;
        }
    }
}
