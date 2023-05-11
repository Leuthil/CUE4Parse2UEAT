using CUE4Parse.UE4.Assets;
using CUE4Parse.Utils;
using CUE4Parse2UEAT.Factory;
using UEATSerializer.UEAT;

namespace CUE4Parse2UEAT.Generation
{
    public static class UAssetUtils
    {
        public static UAsset? CreateUAsset(IPackage assetPackage)
        {
            if (assetPackage == null)
            {
                return null;
            }

            var assetObject = FindAssetObject(assetPackage);

            if (assetObject == null)
            {
                return null;
            }

            var packageObjectRepository = new PackageObjectRepository();
            var packageObjectFactory = CreatePackageObjectFactory(assetPackage, packageObjectRepository);

            if (packageObjectFactory == null)
            {
                return null;
            }

            var context = new GenerationContext(assetPackage, assetObject, packageObjectRepository, packageObjectFactory);

            var uasset = new UAsset();
            uasset.PackageName = assetPackage.Name;
            uasset.ClassName = GetAssetClassName(assetObject);
            uasset.ObjectName = uasset.ClassName.EndsWith("Blueprint") ? assetObject.Name.SubstringBeforeLast("_C") : assetObject.Name;
            uasset.UObjectAsset = UObjectUtils.CreateUObject(assetObject, context);

            if (uasset.UObjectAsset == null)
            {
                return null;
            }

            context.PackageObjectFactory.ProcessImports(assetPackage);
            context.PackageObjectFactory.ProcessExports(assetPackage);
            uasset.ImportPackageObjects = context.PackageObjectRepository.PackageObjects.Where(p => p is ImportPackageObject);
            uasset.ExportPackageObjects = context.PackageObjectRepository.PackageObjects.Where(p => p is ExportPackageObject);

            return uasset;
        }

        public static CUE4Parse.UE4.Assets.Exports.UObject? FindAssetObject(IPackage package)
        {
            if (package == null)
            {
                return null;
            }

            var name = package.Name.SubstringAfterLast('/');
            var uobject = package.GetExportOrNull(name);

            if (uobject == null)
            {
                uobject = package.GetExportOrNull(name + "_C");
            }

            return uobject;
        }

        private static readonly Dictionary<string, string> _cookedClassNameToAssetClassName = new Dictionary<string, string>()
        {
            { "BlueprintGeneratedClass", "Blueprint" },
            { "WidgetBlueprintGeneratedClass", "WidgetBlueprint" },
            { "AnimBlueprintGeneratedClass" , "AnimBlueprint"},
        };

        public static string GetAssetClassName(CUE4Parse.UE4.Assets.Exports.UObject uobject)
        {
            string? className = uobject?.Class?.Name;

            if (className == null)
            {
                return string.Empty;
            }

            if (_cookedClassNameToAssetClassName.TryGetValue(className, out string? assetClassName))
            {
                return assetClassName ?? string.Empty;
            }

            return className;
        }

        private static IPackageObjectFactory? CreatePackageObjectFactory(IPackage package, PackageObjectRepository packageObjectRepository)
        {
            if (package is IoPackage ioPackage)
            {
                return new IoPackageObjectFactory(ioPackage, packageObjectRepository);
            }

            return null;
        }
    }
}
