namespace CUE4Parse2UEAT.Factory
{
    public interface IUObjectFactory
    {
        int Priority { get; }
        UEATSerializer.UEAT.UObject? CreateUObject(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context);
        bool CanHandle(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context);
    }
}
