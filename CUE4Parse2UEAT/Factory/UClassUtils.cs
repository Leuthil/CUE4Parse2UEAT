﻿using CUE4Parse.UE4.Assets;

namespace CUE4Parse2UEAT.Factory
{
    public static class UClassUtils
    {
        public static void PopulateUClassData(CUE4Parse.UE4.Objects.UObject.UClass cue4ParseUClass,
            UEATSerializer.UEAT.UClass ueatUClass, IoPackage package)
        {
            ueatUClass.ClassFlags = cue4ParseUClass.ClassFlags;
            ueatUClass.ClassWithin = PackageObjectUtils.CreatePackageObject(cue4ParseUClass.ClassWithin.ResolvedObject, package);
            ueatUClass.ClassConfigName = cue4ParseUClass.ClassConfigName.Text;

            foreach (var implInterface in cue4ParseUClass.Interfaces)
            {
                if (implInterface == null)
                {
                    continue;
                }

                var implementedInterface = new UEATSerializer.UEAT.FImplementedInterface();

                implementedInterface.Class = PackageObjectUtils.CreatePackageObject(implInterface.Class.ResolvedObject, package);
                implementedInterface.PointerOffset = implInterface.PointerOffset;
                implementedInterface.bImplementedByK2 = implInterface.bImplementedByK2;

                ueatUClass.Interfaces.Add(implementedInterface);
            }

            ueatUClass.ClassDefaultObject = PackageObjectUtils.CreatePackageObject(cue4ParseUClass.ClassDefaultObject.ResolvedObject, package);
        }
    }
}