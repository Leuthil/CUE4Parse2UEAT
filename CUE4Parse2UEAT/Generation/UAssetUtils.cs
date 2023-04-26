using CUE4Parse.UE4.Assets;
using CUE4Parse.Utils;
using UEATSerializer.UEAT;
using UObject = CUE4Parse.UE4.Assets.Exports.UObject;

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
            var context = new GenerationContext(assetPackage, assetObject);

            var uasset = new UAsset();
            uasset.PackageName = assetPackage.Name;
            uasset.ObjectName = assetObject.Name;
            uasset.ClassName = GetAssetClassName(assetObject);
            uasset.UObjectAsset = UObjectUtils.CreateUObject(assetObject, context);

            assetPackage.GetExports().Select(e => context.PackageObjectFactory.CreatePackageObject(e));

            uasset.ImportPackageObjects = context.PackageObjectRepository.PackageObjects.Where(p => p is ImportPackageObject);
            uasset.ExportPackageObjects = context.PackageObjectRepository.PackageObjects.Where(p => p is ExportPackageObject);

            return uasset;
        }

        public static UObject? FindAssetObject(IPackage package)
        {
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

        public static string GetAssetClassName(UObject uobject)
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
    }
}
