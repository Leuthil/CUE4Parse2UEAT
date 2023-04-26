using CUE4Parse.UE4.Assets;
using CUE4Parse2UEAT.Generation;
using UEATSerializer.UEAT;

namespace CUE4Parse2UEAT.Factory
{
    public abstract class AbstractPackageObjectFactory<T> : IPackageObjectFactory where T : IPackage
    {
        protected T Package { get; init; }
        protected PackageObjectRepository Repository { get; init; }

        public AbstractPackageObjectFactory(T package, PackageObjectRepository repository)
        {
            Package = package;
            Repository = repository;
        }

        public abstract PackageObject? CreatePackageObject(CUE4Parse.UE4.Assets.Exports.UObject? uobject);
    }
}
