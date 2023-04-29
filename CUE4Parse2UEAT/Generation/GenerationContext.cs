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

        public GenerationContext(IPackage package, CUE4Parse.UE4.Assets.Exports.UObject assetObject,
            PackageObjectRepository packageObjectRepository, IPackageObjectFactory packageObjectFactory)
        {
            Package = package;
            AssetObject = assetObject;
            PackageObjectRepository = packageObjectRepository;
            PackageObjectFactory = packageObjectFactory;
        }
    }
}
