using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse2UEAT.Generation;
using UEATSerializer.UEAT;

namespace CUE4Parse2UEAT.Factory
{
    public abstract class AbstractPackageObjectFactory<T> : IPackageObjectFactory where T : class, IPackage
    {
        protected T Package { get; init; }
        protected PackageObjectRepository Repository { get; init; }

        public AbstractPackageObjectFactory(T package, PackageObjectRepository repository)
        {
            Package = package;
            Repository = repository;
        }

        public abstract PackageObject? CreatePackageObject(FPackageIndex? fPackageIndex);
        public abstract void ProcessImports(T package);
        public abstract void ProcessExports(T package);

        public virtual void ProcessImports(IPackage package)
        {
            ProcessImports(package as T);
        }

        public virtual void ProcessExports(IPackage package)
        {
            ProcessExports(package as T);
        }
    }
}
