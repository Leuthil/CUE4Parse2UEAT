namespace UEATSerializer.UEAT
{
    public readonly struct UObjectIdentifier : IEquatable<UObjectIdentifier>
    {
        public string PackageName { get; init; }
        public string ObjectName { get; init; }
        public string OuterName { get; init; }

        public UObjectIdentifier(string packageName, string objectName, string outerName)
        {
            PackageName = packageName ?? string.Empty;
            ObjectName = objectName ?? string.Empty;
            OuterName = outerName ?? string.Empty;
        }

        public bool Equals(UObjectIdentifier other)
        {
            return string.Equals(PackageName, other.PackageName) && string.Equals(ObjectName, other.ObjectName)
                && string.Equals(OuterName, other.OuterName);
        }

        public override bool Equals(object? obj)
        {
            return obj is UObjectIdentifier id && this.Equals(id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PackageName, ObjectName, OuterName);
        }

        public static bool operator ==(UObjectIdentifier x, UObjectIdentifier y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(UObjectIdentifier x, UObjectIdentifier y)
        {
            return !(x == y);
        }

        public static bool Equals(UObjectIdentifier? x, UObjectIdentifier? y)
        {
            if (x == null)
            {
                return y == null;
            }

            if (y == null)
            {
                return false;
            }

            return x.Value.Equals(y.Value);
        }

        public override string ToString() => $"{PackageName}.{OuterName}.{ObjectName}";
    }
}
