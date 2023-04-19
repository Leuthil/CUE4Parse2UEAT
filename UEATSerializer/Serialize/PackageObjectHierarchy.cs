using UEATSerializer.UEAT;

namespace UEATSerializer.Serializer
{
    public class PackageObjectHierarchy
    {
        public IEnumerable<KeyValuePair<UObject, int>> PackageObjects => PackageObjectToIndexDict;
        public UObject PackageUObjectAsset { get; protected set; }

        protected Dictionary<UObject, int> PackageObjectToIndexDict = new Dictionary<UObject, int>();
        protected int currentObjectHierarchyIndex = 0;

        protected Dictionary<UObjectIdentifier, UObject> idToUObject = new Dictionary<UObjectIdentifier, UObject>();
        protected Dictionary<UObjectIdentifier, int> idToIndex = new Dictionary<UObjectIdentifier, int>();

        public PackageObjectHierarchy(UObject packageUObjectAsset)
        {
            PackageUObjectAsset = packageUObjectAsset;
        }

        /// <summary>
        /// Returns the object index, or -1 if not found.
        /// </summary>
        public int GetObjectIndex(UObjectIdentifier? id)
        {
            if (id == null)
            {
                return -1;
            }

            return GetObjectIndex(id.Value);
        }

        /// <summary>
        /// Returns the object index, or -1 if not found.
        /// </summary>
        public int GetObjectIndex(UObjectIdentifier id)
        {
            if (idToIndex.TryGetValue(id, out int index))
            {
                return index;
            }

            return -1;
        }

        /// <summary>
        /// Adds the given UObject and returns its newly assigned object index.
        /// If the given UObject has already been added, its previously assigned object index is returned.
        /// If the given UObject is null, -1 is returned.
        /// </summary>
        public int AddUnique(UObject uobject)
        {
            if (uobject == null)
            {
                return -1;
            }

            int index = GetObjectIndex(uobject.Id);

            if (index != -1)
            {
                return index;
            }

            index = currentObjectHierarchyIndex++;
            idToUObject.Add(uobject.Id, uobject);
            idToIndex.Add(uobject.Id, index);
            PackageObjectToIndexDict.Add(uobject, index);

            return index;
        }
    }
}
