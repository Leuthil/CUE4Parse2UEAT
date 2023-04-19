using CUE4Parse.UE4.Assets;

namespace CUE4Parse2UEAT.Factory
{
    public interface IUObjectFactory
    {
        int Priority { get; }
        UEATSerializer.UEAT.UObject? CreateUObject(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, IoPackage package);
        bool CanHandle(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, IoPackage package);
    }
}
