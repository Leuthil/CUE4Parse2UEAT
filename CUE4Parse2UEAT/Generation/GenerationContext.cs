using CUE4Parse.UE4.Assets;
using UEATSerializer.UEAT;

namespace CUE4Parse2UEAT.Generation
{
    public class GenerationContext
    {
        public IoPackage Package { get; }
        public CUE4Parse.UE4.Assets.Exports.UObject AssetObject { get; }

        protected Dictionary<UObjectIdentifier, PackageObject> idToPackageObjectDict = new Dictionary<UObjectIdentifier, PackageObject>();

        public GenerationContext(IoPackage package, CUE4Parse.UE4.Assets.Exports.UObject assetObject)
        {
            Package = package;
            AssetObject = assetObject;
        }

        public bool ContainsPackageObject(UObjectIdentifier id)
        {
            return idToPackageObjectDict.ContainsKey(id);
        }

        public PackageObject? GetPackageObject(UObjectIdentifier id)
        {
            if (idToPackageObjectDict.TryGetValue(id, out PackageObject? packageObject))
            {
                return packageObject;
            }

            return null;
        }

        public bool AddPackageObject(PackageObject packageObject)
        {
            if (ContainsPackageObject(packageObject.Id))
            {
                return false;
            }

            idToPackageObjectDict.Add(packageObject.Id, packageObject);

            return true;
        }
    }
}
