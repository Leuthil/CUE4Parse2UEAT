using CUE4Parse2UEAT.Factory;

namespace CUE4Parse2UEAT.Generation
{
    public static class UEnumUtils
    {
        public static void PopulateUEnumData(CUE4Parse.UE4.Objects.UObject.UEnum cue4ParseUEnum,
            UEATSerializer.UEAT.UEnum ueatUEnum, IPackageObjectFactory packageObjectFactory)
        {
            foreach (var name in cue4ParseUEnum.Names)
            {
                ueatUEnum.Names.Add(new UEATSerializer.UEAT.UEnum.EnumName()
                {
                    Name = name.Item1.Text,
                    Value = name.Item2
                });
            }

            ueatUEnum.CppForm = (byte)cue4ParseUEnum.CppForm;
        }
    }
}
