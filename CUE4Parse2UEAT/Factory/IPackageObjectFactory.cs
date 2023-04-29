namespace CUE4Parse2UEAT.Factory
{
    public interface IPackageObjectFactory
    {
        UEATSerializer.UEAT.PackageObject? CreatePackageObject(CUE4Parse.UE4.Objects.UObject.FPackageIndex? fPackageIndex);
        void ProcessImports(CUE4Parse.UE4.Assets.IPackage package);
        void ProcessExports(CUE4Parse.UE4.Assets.IPackage package);
    }
}
