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

        public override PackageObject? CreatePackageObject(CUE4Parse.UE4.Assets.Exports.UObject? uobject)
        {
            if (uobject == null)
            {
                return null;
            }

            // import
            if (!Package.Name.Equals(GetPackage(uobject)?.Name))
            {
                return CreateImportPackageObject(uobject);
            }

            return CreateExportPackageObject(uobject);
        }

        public PackageObject CreateImportPackageObject(CUE4Parse.UE4.Assets.Exports.UObject uobject)
        {
            if (uobject is IPackage)
            {
                return CreatePackageImportPackageObject(uobject.Name);
            }

            if (uobject is UScriptClass uscriptClass)
            {
                return CreateScriptClassImportPackageObject(uscriptClass);
            }

            var packageObject = new ImportPackageObject();
            packageObject.PackageName = GetPackage(uobject)?.Name;
            packageObject.ObjectName = uobject.Name;
            packageObject.OuterName = uobject.Outer?.Name;

            if (Repository.Contains(packageObject.Id))
            {
                return Repository.Get(packageObject.Id);
            }

            // add before resolving any other objects
            Repository.Add(packageObject);

            packageObject.ClassPackage = GetClassPackageName(uobject);
            packageObject.ClassName = uobject.Class?.Name;
            packageObject.Outer = CreatePackageObject(uobject?.Outer);

            CreatePackageImportPackageObject(packageObject.PackageName);

            return packageObject;
        }

        public PackageObject CreateScriptClassImportPackageObject(UScriptClass uscriptClass, string? packageName = null)
        {
            var packageObject = new ImportPackageObject();
            packageObject.PackageName = packageName ?? uscriptClass.Owner?.Name;
            packageObject.ObjectName = uscriptClass.Name;
            packageObject.ClassName = uscriptClass.Class?.Name;
            packageObject.ClassPackage = GetClassPackageName(uscriptClass);
            packageObject.OuterName = uscriptClass.Outer?.Name;

            // fixups
            
            if (packageObject.PackageName == null)
            {
                packageObject.PackageName = "/Script/CoreUObject";
            }

            if (packageObject.ClassPackage == null)
            {
                packageObject.ClassPackage = "/Script/CoreUObject";
            }

            if (packageObject.ClassName == null)
            {
                packageObject.ClassName = "Class";
            }

            // Workaround because CUE4Parse cannot differentiate between UScriptStruct and UClass, so everything is classified as "Class".
            // This relies on .usmap mappings file being provided to CUE4Parse provider.
            // See code comment within CUE4Parse.UE4.Assets.ResolvedScriptObject.
            if (Package.Mappings != null
                && Package.Mappings.Types.TryGetValue(uscriptClass.Name, out var type))
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

            packageObject.OuterName = packageObject.PackageName;

            if (Repository.Contains(packageObject.Id))
            {
                return Repository.Get(packageObject.Id);
            }

            // add before resolving any other objects
            Repository.Add(packageObject);

            // class outer should be a package... I think
            packageObject.Outer = CreatePackageImportPackageObject(packageObject.PackageName);

            CreatePackageImportPackageObject(packageObject.PackageName);

            return packageObject;
        }

        public PackageObject? CreatePackageImportPackageObject(string objectName)
        {
            var packageObject = new ImportPackageObject();
            packageObject.PackageName = "/Script/CoreUObject";
            packageObject.ObjectName = objectName;
            packageObject.OuterName = "/Script/CoreUObject";

            if (Repository.Contains(packageObject.Id))
            {
                return Repository.Get(packageObject.Id);
            }

            // add before resolving any other objects
            Repository.Add(packageObject);

            packageObject.ClassPackage = "/Script/CoreUObject";
            packageObject.ClassName = "Package";

            return packageObject;
        }

        public ExportPackageObject? CreateExportPackageObject(CUE4Parse.UE4.Assets.Exports.UObject exportObject)
        {
            var packageObject = new ExportPackageObject();

            // set this early so the UObjectIdentifier is valid before adding to the context
            UObjectUtils.PopulateUObjectIdentification(exportObject, packageObject);

            if (Repository.Contains(packageObject.Id))
            {
                return Repository.Get(packageObject.Id) as ExportPackageObject;
            }

            // add before resolving any other objects
            Repository.Add(packageObject);

            UObjectUtils.PopulateUObjectProperties(exportObject, packageObject, this);

            if (exportObject?.Class is UScriptClass uscriptClass)
            {
                packageObject.ObjectClass = CreateScriptClassImportPackageObject(uscriptClass, GetClassPackageName(exportObject));
            }
            else
            {
                packageObject.ObjectClass = CreatePackageObject(exportObject?.Class);
            }

            packageObject.ObjectFlags = (int)exportObject.Flags;
            packageObject.Outer = CreatePackageObject(exportObject?.Outer);

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

        protected static CUE4Parse.UE4.Assets.Exports.UObject? GetPackage(CUE4Parse.UE4.Assets.Exports.UObject? uobject)
        {
            var package = uobject;

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
