using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.IO.Objects;
using CUE4Parse.UE4.Objects.UObject;
using UEATSerializer.UEAT;

namespace CUE4Parse2UEAT.Generation
{
    public static class PackageObjectUtils
    {
        public static PackageObject? CreatePackageObject(FPackageObjectIndex? packageObjectIndex, GenerationContext context)
        {
            if (packageObjectIndex == null || packageObjectIndex.Value.IsNull)
            {
                return null;
            }

            var resolvedObject = context.Package.ResolveObjectIndex(packageObjectIndex.Value);

            return CreatePackageObject(resolvedObject, context);
        }

        public static PackageObject? CreatePackageObject(ResolvedObject? resolvedObject, GenerationContext context)
        {
            if (resolvedObject == null)
            {
                return null;
            }

            var objectName = resolvedObject.Name.Text;
            var objectPackage = GetPackage(resolvedObject);
            var packageName = objectPackage?.Name.Text;

            var objectId = new UObjectIdentifier(packageName, objectName, resolvedObject?.Outer?.Name.Text);

            if (context.ContainsPackageObject(objectId))
            {
                return context.GetPackageObject(objectId);
            }

            // import
            if (!context.Package.Name.Equals(packageName))
            {
                return CreateImportPackageObject(packageName, objectName, resolvedObject, context);
            }

            if (!resolvedObject.TryLoad(out var exportObject))
            {
                return null;
            }

            return CreateExportPackageObject(exportObject, resolvedObject.Class, resolvedObject.Outer, (int)resolvedObject.Object.Value.Flags, context);
        }

        private static PackageObject CreateImportPackageObject(string? packageName, string objectName, ResolvedObject resolvedObject, GenerationContext context)
        {
            var classPackage = resolvedObject.Class?.Outer?.Name.Text;
            var className = resolvedObject.Class?.Name.Text;

            // fixups

            if (resolvedObject.Class?.Outer == null)
            {
                classPackage = resolvedObject.Outer?.Name.Text;
            }

            // CDOs
            if ("Class".Equals(className) && objectName.StartsWith("Default__"))
            {
                className = objectName.Substring("Default__".Length);
            }

            // class (should this be combined with the above fixup for CDOs?)
            if ("Class".Equals(className))
            {
                classPackage = "/Script/CoreUObject";

                // workaround because CUE4Parse cannot differentiate between UScriptStruct and UClass, so everything is classified as "Class"
                // see code comment within CUE4Parse.UE4.Assets.ResolvedScriptObject
                if (context.Package?.Provider?.MappingsForGame != null
                    && context.Package.Provider.MappingsForGame.Types.TryGetValue(objectName, out var type))
                {
                    var superType = type;

                    while (superType?.Super.Value != null)
                    {
                        superType = superType.Super.Value;
                    }

                    // no idea if this is a legit check, but it seems to work so far
                    bool isStruct = !"Object".Equals(superType.Name);
                    if (isStruct)
                    {
                        className = "ScriptStruct";
                    }
                }

            }

            // packages
            // if Outer is null then this is a package
            if (resolvedObject.Outer == null)
            {
                classPackage = "/Script/CoreUObject";
                className = "Package";
            }

            var import = new ImportPackageObject();
            import.PackageName = packageName;
            import.ObjectName = objectName;
            import.OuterName = resolvedObject?.Outer?.Name.Text;

            // add before resolving any other objects
            context.AddPackageObject(import);

            import.ClassPackage = classPackage;
            import.ClassName = className;
            import.Outer = CreatePackageObject(resolvedObject?.Outer, context);

            return import;
        }

        public static PackageObject? CreatePackageObject(FExportMapEntry exportMapEntry, GenerationContext context)
        {
            var objectName = CreateFNameFromMappedName(exportMapEntry.ObjectName, context.Package.GlobalData.GlobalNameMap, context.Package.NameMap).Text;
            var packageName = context.Package.Name;
            var outerObject = context.Package.ResolveObjectIndex(exportMapEntry.OuterIndex);
            
            var objectId = new UObjectIdentifier(packageName, objectName, outerObject?.Name.Text);

            if (context.ContainsPackageObject(objectId))
            {
                return context.GetPackageObject(objectId);
            }

            var exportObject = context.Package.GetExport(objectName);
            var classObject = context.Package.ResolveObjectIndex(exportMapEntry.ClassIndex);

            return CreateExportPackageObject(exportObject, classObject, outerObject, (int)exportMapEntry.ObjectFlags, context);
        }

        public static ExportPackageObject CreateExportPackageObject(CUE4Parse.UE4.Assets.Exports.UObject exportObject, ResolvedObject? objectClass,
            ResolvedObject? outer, int objectFlags, GenerationContext context)
        {
            var export = new ExportPackageObject();

            // set this early so the UObjectIdentifier is valid before adding to the context
            UObjectUtils.PopulateUObjectIdentification(exportObject, export, context);

            // add before resolving any other objects
            context.AddPackageObject(export);

            UObjectUtils.PopulateUObjectProperties(exportObject, export, context);

            export.ObjectClass = CreatePackageObject(objectClass, context);
            export.ObjectFlags = objectFlags;
            export.Outer = CreatePackageObject(outer, context);

            return export;
        }

        public static ResolvedObject? GetPackage(ResolvedObject? resolvedObject)
        {
            var package = resolvedObject;

            while (package.Outer != null)
            {
                package = package.Outer;
            }

            return package;
        }

        public static FName CreateFNameFromMappedName(FMappedName mappedName, FNameEntrySerialized[] globalNameMap, FNameEntrySerialized[] nameMap) =>
            new(mappedName, mappedName.IsGlobal ? globalNameMap : nameMap);
    }
}
