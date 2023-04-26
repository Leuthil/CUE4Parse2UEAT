using CUE4Parse2UEAT.Generation;

namespace CUE4Parse2UEAT.Factory
{
    public interface IPackageObjectFactory
    {
        UEATSerializer.UEAT.PackageObject? CreatePackageObject(CUE4Parse.UE4.Assets.Exports.UObject? uobject);
    }
}
