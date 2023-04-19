using UEATSerializer.Serializer;

namespace UEATSerializer.UEAT
{
    // Can reference one or more UObjects
    internal interface IUObjectPointer
    {
        /// <summary>
        /// Return the indices of all referenced objects. May include -1 which means the object is null.
        /// </summary>
        int[] ResolveObjectReferences(PackageObjectHierarchy objectHierarchy);
    }
}
