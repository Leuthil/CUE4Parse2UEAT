﻿using CUE4Parse2UEAT.Factory;

namespace CUE4Parse2UEAT.Generation
{
    public static class UStructUtils
    {
        public static void PopulateUStructData(CUE4Parse.UE4.Objects.UObject.UStruct cue4ParseUStruct,
            UEATSerializer.UEAT.UStruct ueatUStruct, IPackageObjectFactory packageObjectFactory)
        {
            ueatUStruct.SuperStruct = packageObjectFactory.CreatePackageObject(cue4ParseUStruct.SuperStruct);

            foreach (var child in cue4ParseUStruct.Children)
            {
                if (!child.TryLoad(out CUE4Parse.UE4.Assets.Exports.UObject? childExport))
                {
                    continue;
                }

                if (childExport is not CUE4Parse.UE4.Objects.UObject.UFunction func)
                {
                    continue;
                }

                var ufunction = UFunctionUtils.CreateUFunction(func, packageObjectFactory);

                if (ufunction == null)
                {
                    continue;
                }

                ueatUStruct.Children.Add(ufunction);
            }

            foreach (var childProperty in cue4ParseUStruct.ChildProperties)
            {
                if (childProperty is not CUE4Parse.UE4.Objects.UObject.FProperty fprop)
                {
                    continue;
                }

                var fproperty = FPropertyUtils.CreateFProperty(fprop, packageObjectFactory);

                if (fproperty == null)
                {
                    continue;
                }

                ueatUStruct.ChildProperties.Add(KeyValuePair.Create(fprop.Name.Text, fproperty));
            }

            // (requires bytecode disassembly, not sure if available) (BytecodeDisassembler.SerializeFunction(struct))
            //ueatUStruct.Script = new List<object>();
        }
    }
}
