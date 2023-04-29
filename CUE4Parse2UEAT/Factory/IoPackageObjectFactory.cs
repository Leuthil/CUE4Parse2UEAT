using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.IO.Objects;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse2UEAT.Generation;
using UEATSerializer.UEAT;

namespace CUE4Parse2UEAT.Factory
{
    public class IoPackageObjectFactory : AbstractPackageObjectFactory<IoPackage>
    {
        public IoPackageObjectFactory(IoPackage package, PackageObjectRepository repository) : base(package, repository)
        {
        }

        public override void ProcessImports(IoPackage package)
        {
            if (package == null)
            {
                return;
            }

            for (int i = 0; i < package.ImportMap.Length; i++)
            {
                var import = package.ImportMap[i];
                CreatePackageObject(package.ResolveObjectIndex(import));
            }
        }

        public override void ProcessExports(IoPackage package)
        {
            if (package == null)
            {
                return;
            }

            for (int i = 0; i < package.ExportMap.Length; i++)
            {
                var exportMapEntry = package.ExportMap[i];
                var exportObject = package.ExportsLazy[i];
                var outerResolvedObject = package.ResolveObjectIndex(exportMapEntry.OuterIndex);
                var classResolvedObject = package.ResolveObjectIndex(exportMapEntry.ClassIndex);

                ExportPackageObject packageObject = new ExportPackageObject();
                packageObject.ObjectName = CreateFNameFromMappedName(exportMapEntry.ObjectName, package).Text;
                packageObject.PackageName = package.Name;
                packageObject.OuterName = package.ResolveObjectIndex(exportMapEntry.OuterIndex)?.Name.Text;

                if (packageObject.OuterName == null)
                {
                    packageObject.OuterName = package.Name;
                }

                if (Repository.Contains(packageObject.Id))
                {
                    continue;
                }

                // add before resolving any other objects
                Repository.Add(packageObject);

                UObjectUtils.PopulateUObjectProperties(exportObject.Value, packageObject, this);

                packageObject.ObjectClass = CreatePackageObject(classResolvedObject);
                packageObject.Outer = CreatePackageObject(outerResolvedObject) ?? CreatePackageImportPackageObject(package.Name);

                continue;
            }
        }

        public override PackageObject? CreatePackageObject(FPackageIndex? fPackageIndex)
        {
            return CreatePackageObject(fPackageIndex?.ResolvedObject);
        }

        public PackageObject? CreatePackageObject(ResolvedObject? resolvedObject)
        {
            if (resolvedObject == null)
            {
                return null;
            }

            var packageName = GetPackage(resolvedObject)?.Name.Text;
            var objectName = resolvedObject.Name.Text;

            if (Package.Name != packageName)
            {
                ImportPackageObject packageObject = new ImportPackageObject();
                packageObject.PackageName = packageName;
                packageObject.ObjectName = objectName;
                packageObject.OuterName = resolvedObject.Outer?.Name.Text;

                if (packageObject.PackageName != null && packageObject.PackageName == packageObject.ObjectName)
                {
                    return CreatePackageImportPackageObject(packageObject.PackageName);
                }

                if (Repository.Contains(packageObject.Id))
                {
                    return Repository.Get(packageObject.Id);
                }

                // add before resolving any other objects
                Repository.Add(packageObject);

                packageObject.Outer = CreatePackageObject(resolvedObject.Outer);
                packageObject.ClassPackage = GetPackage(resolvedObject.Class)?.Name.Text;
                packageObject.ClassName = resolvedObject.Class?.Name.Text;

                if (packageObject.ClassName == "Class")
                {
                    packageObject.ClassPackage = "/Script/CoreUObject";

                    // Workaround because CUE4Parse cannot differentiate between UScriptStruct and UClass, so everything is classified as "Class".
                    // This relies on .usmap mappings file being provided to CUE4Parse provider.
                    // See code comment within CUE4Parse.UE4.Assets.ResolvedScriptObject.
                    if (Package.Mappings != null
                        && Package.Mappings.Types.TryGetValue(resolvedObject.Name.Text, out var type))
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
                            packageObject.ClassName = "ScriptStruct";
                        }
                    }
                }

                CreateClassImportPackageObject(packageObject.ClassPackage, packageObject.ClassName);

                return packageObject;
            }
            else
            {
                ExportPackageObject packageObject = new ExportPackageObject();
                packageObject.PackageName = GetPackage(resolvedObject)?.Name.Text;
                packageObject.ObjectName = resolvedObject.Name.Text;
                packageObject.OuterName = resolvedObject.Outer?.Name.Text;
                packageObject.ObjectFlags = (int)resolvedObject.Load()?.Flags;

                if (Repository.Contains(packageObject.Id))
                {
                    return Repository.Get(packageObject.Id);
                }

                // add before resolving any other objects
                Repository.Add(packageObject);

                UObjectUtils.PopulateUObjectProperties(resolvedObject.Load(), packageObject, this);

                packageObject.ObjectClass = CreatePackageObject(resolvedObject.Class);
                packageObject.Outer = CreatePackageObject(resolvedObject.Outer);

                return packageObject;
            }
        }

        public PackageObject? CreateClassImportPackageObject(string classPackageName, string className)
        {
            ImportPackageObject packageObject = new ImportPackageObject();
            packageObject.PackageName = classPackageName;
            packageObject.ObjectName = className;
            packageObject.ClassPackage = classPackageName;
            packageObject.ClassName = className;
            packageObject.OuterName = classPackageName;

            if (Repository.Contains(packageObject.Id))
            {
                return Repository.Get(packageObject.Id);
            }

            // add before resolving any other objects
            Repository.Add(packageObject);

            packageObject.Outer = CreatePackageImportPackageObject(classPackageName);

            return packageObject;
        }

        public PackageObject? CreatePackageImportPackageObject(string packageName)
        {
            var packageObject = new ImportPackageObject();
            packageObject.PackageName = "/Script/CoreUObject";
            packageObject.ObjectName = packageName;
            packageObject.OuterName = "/Script/CoreUObject";

            if (Repository.Contains(packageObject.Id))
            {
                return Repository.Get(packageObject.Id);
            }

            // add before resolving any other objects
            Repository.Add(packageObject);

            packageObject.ClassPackage = "/Script/CoreUObject";
            packageObject.ClassName = "Package";
            
            // packageObject.Outer intentionally left null

            return packageObject;
        }

        protected static string? GetClassPackageName(CUE4Parse.UE4.Assets.Exports.UObject uobject)
        {
            string? classPackageName = uobject?.Class?.Owner?.Name;

            if (classPackageName != null)
            {
                return classPackageName;
            }

            // Workaround for IoPackage
            // LongerWarrior's CUE4Parse fork does this: See how "ClassPath" is serialized within CUE4Parse.UE4.Assets.Exports.UObject
            if (uobject.ClassIndex != null && uobject.Owner is IoPackage owner)
            {
                var index = (FPackageObjectIndex)uobject.ClassIndex;

                if (index.IsScriptImport)
                {
                    if (owner.GlobalData.ScriptObjectEntriesMap.TryGetValue(index, out var scriptObjectEntry))
                    {
                        if (!scriptObjectEntry.OuterIndex.IsNull)
                        {
                            if (owner.GlobalData.ScriptObjectEntriesMap.TryGetValue(scriptObjectEntry.OuterIndex, out var outerScriptObjectEntry))
                            {
                                return CreateFNameFromMappedName(outerScriptObjectEntry.ObjectName, owner).Text;
                            }
                        }
                    }
                }
            }

            return classPackageName;
        }

        protected static ResolvedObject? GetPackage(ResolvedObject? resolvedObject)
        {
            var package = resolvedObject;

            while (package?.Outer != null)
            {
                package = package.Outer;
            }

            return package;
        }

        // from CUE4Parse.UE4.Assets.IoPackage
        protected static FName CreateFNameFromMappedName(FMappedName mappedName, IoPackage package) =>
            new(mappedName, mappedName.IsGlobal ? package.GlobalData.GlobalNameMap : package.NameMap);
    }
}
