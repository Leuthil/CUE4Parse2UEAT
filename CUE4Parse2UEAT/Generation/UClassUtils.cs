using CUE4Parse2UEAT.Factory;

namespace CUE4Parse2UEAT.Generation
{
    public static class UClassUtils
    {
        public static void PopulateUClassData(CUE4Parse.UE4.Objects.UObject.UClass cue4ParseUClass,
            UEATSerializer.UEAT.UClass ueatUClass, IPackageObjectFactory packageObjectFactory)
        {
            ueatUClass.ClassFlags = cue4ParseUClass.ClassFlags;
            ueatUClass.ClassWithin = packageObjectFactory.CreatePackageObject(cue4ParseUClass.ClassWithin?.ResolvedObject?.Load());
            ueatUClass.ClassConfigName = cue4ParseUClass.ClassConfigName.Text;

            foreach (var implInterface in cue4ParseUClass.Interfaces)
            {
                if (implInterface == null)
                {
                    continue;
                }

                var implementedInterface = new UEATSerializer.UEAT.FImplementedInterface();

                implementedInterface.Class = packageObjectFactory.CreatePackageObject(implInterface.Class?.ResolvedObject?.Load());
                implementedInterface.PointerOffset = implInterface.PointerOffset;
                implementedInterface.bImplementedByK2 = implInterface.bImplementedByK2;

                ueatUClass.Interfaces.Add(implementedInterface);
            }

            ueatUClass.ClassDefaultObject = packageObjectFactory.CreatePackageObject(cue4ParseUClass.ClassDefaultObject?.ResolvedObject?.Load());
        }
    }
}
