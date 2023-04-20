﻿using CUE4Parse.UE4.Assets;
using CUE4Parse.Utils;
using UEATSerializer.UEAT;
using UObject = CUE4Parse.UE4.Assets.Exports.UObject;

namespace CUE4Parse2UEAT.Factory
{
    public static class UAssetUtils
    {
        public static UAsset CreateUAsset(IoPackage assetPackage)
        {
            var assetObject = FindAssetObject(assetPackage);

            var uasset = new UAsset();
            uasset.PackageName = assetPackage.Name;
            uasset.ObjectName = assetPackage.Name.SubstringAfterLast('/');
            uasset.ClassName = GetAssetClassName(assetObject);
            uasset.UObjectAsset = UObjectUtils.CreateUObject(assetObject, assetPackage);

            uasset.ImportPackageObjects = assetPackage.ImportMap.Select(i => PackageObjectUtils.CreatePackageObject(i, assetPackage));
            uasset.ExportPackageObjects = assetPackage.ExportMap.Select(e => PackageObjectUtils.CreatePackageObject(e, assetPackage));

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
            { "BlueprintGeneratedClass", "Blueprint" }
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
