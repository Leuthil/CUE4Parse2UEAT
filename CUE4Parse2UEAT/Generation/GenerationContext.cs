using CUE4Parse.UE4.Assets;
using CUE4Parse2UEAT.Factory;

namespace CUE4Parse2UEAT.Generation
{
    public class GenerationContext
    {
        public IPackage Package { get; init; }
        public CUE4Parse.UE4.Assets.Exports.UObject AssetObject { get; init; }
        public PackageObjectRepository PackageObjectRepository { get; init; }
        public IPackageObjectFactory PackageObjectFactory { get; init; }

        public GenerationContext(IPackage package, CUE4Parse.UE4.Assets.Exports.UObject assetObject)
        {
            Package = package;
            AssetObject = assetObject;
            PackageObjectRepository = new PackageObjectRepository();
            PackageObjectFactory = CreatePackageObjectFactory(package, PackageObjectRepository) ?? throw new ArgumentException($"Package \"{package}\" of type " +
                $"\"{package?.GetType()?.Name}\" is not supported by any registered {nameof(IPackageObjectFactory)}", nameof(package));
        }

        protected static IPackageObjectFactory? CreatePackageObjectFactory(IPackage package, PackageObjectRepository packageObjectRepository)
        {
            if (package is IoPackage ioPackage)
            {
                return new IoPackageObjectFactory(ioPackage, packageObjectRepository);
            }

            return null;
        }
    }
}
