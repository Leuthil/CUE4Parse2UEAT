using CUE4Parse.UE4.Assets;
using CUE4Parse.Utils;
using UEATSerializer.UEAT;
using UObject = CUE4Parse.UE4.Assets.Exports.UObject;

namespace CUE4Parse2UEAT.Generation
{
    public static class UAssetUtils
    {
        public static UAsset? CreateUAsset(IoPackage assetPackage)
        {
            if (assetPackage == null)
            {
                return null;
            }

            var assetObject = FindAssetObject(assetPackage);
            var context = new GenerationContext(assetPackage, assetObject);

            var uasset = new UAsset();
            uasset.PackageName = assetPackage.Name;
            uasset.ObjectName = assetPackage.Name.SubstringAfterLast('/');
            uasset.ClassName = GetAssetClassName(assetObject);
            uasset.UObjectAsset = UObjectUtils.CreateUObject(assetObject, context);

            uasset.ImportPackageObjects = assetPackage.ImportMap.Select(i => PackageObjectUtils.CreatePackageObject(i, context));
            uasset.ExportPackageObjects = assetPackage.ExportMap.Select(e => PackageObjectUtils.CreatePackageObject(e, context));

            return uasset;
        }

        public static UObject? FindAssetObject(IoPackage package)
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
