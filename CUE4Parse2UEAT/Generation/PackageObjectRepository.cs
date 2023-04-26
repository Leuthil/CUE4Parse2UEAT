using UEATSerializer.UEAT;

namespace CUE4Parse2UEAT.Generation
{
    public class PackageObjectRepository
    {
        public IEnumerable<PackageObject> PackageObjects => idToPackageObjectDict.Values;
        protected Dictionary<UObjectIdentifier, PackageObject> idToPackageObjectDict = new Dictionary<UObjectIdentifier, PackageObject>();

        public bool Contains(UObjectIdentifier id)
        {
            return idToPackageObjectDict.ContainsKey(id);
        }

        public PackageObject? Get(UObjectIdentifier id)
        {
            if (idToPackageObjectDict.TryGetValue(id, out PackageObject? packageObject))
            {
                return packageObject;
            }

            return null;
        }

        public bool Add(PackageObject packageObject)
        {
            if (Contains(packageObject.Id))
            {
                return false;
            }

            idToPackageObjectDict.Add(packageObject.Id, packageObject);

            return true;
        }
    }
}
