namespace UEATSerializer.UEAT
{
    public readonly struct UObjectIdentifier : IEquatable<UObjectIdentifier>
    {
        public string PackageName { get; init; }
        public string ObjectName { get; init; }

        public UObjectIdentifier(string packageName, string objectName)
        {
            PackageName = packageName;
            ObjectName = objectName;
        }

        public bool Equals(UObjectIdentifier other)
        {
            return string.Equals(PackageName, other.PackageName) && string.Equals(ObjectName, other.ObjectName);
        }

        public override bool Equals(object? obj)
        {
            return obj is UObjectIdentifier id && this.Equals(id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PackageName, ObjectName);
        }

        public static bool operator ==(UObjectIdentifier x, UObjectIdentifier y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(UObjectIdentifier x, UObjectIdentifier y)
        {
            return !(x == y);
        }

        public override string ToString() => $"{PackageName}.{ObjectName}";
    }
}
