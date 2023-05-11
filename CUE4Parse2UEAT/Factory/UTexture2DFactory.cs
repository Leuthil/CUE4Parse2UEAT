using CUE4Parse2UEAT.Generation;
using SkiaSharp;

namespace CUE4Parse2UEAT.Factory
{
    internal class UTexture2DFactory : IUObjectFactory
    {
        public int Priority => 0;

        public bool CanHandle(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context)
        {
            return assetObject is CUE4Parse.UE4.Assets.Exports.Texture.UTexture2D;
        }

        public UEATSerializer.UEAT.UObject? CreateUObject(CUE4Parse.UE4.Assets.Exports.UObject? assetObject, GenerationContext context)
        {
            if (assetObject is not CUE4Parse.UE4.Assets.Exports.Texture.UTexture2D cue4parseTexture2D)
            {
                return null;
            }

            UEATSerializer.UEAT.UTexture2D texture2D = new UEATSerializer.UEAT.UTexture2D();

            UObjectUtils.PopulateUObjectIdentification(cue4parseTexture2D, texture2D);
            UObjectUtils.PopulateUObjectProperties(cue4parseTexture2D, texture2D, context.PackageObjectFactory);

            texture2D.TextureWidth = 1;
            texture2D.TextureHeight = 1;
            texture2D.TextureDepth = 1;
            texture2D.NumSlices = 1;
            texture2D.CookedPixelFormat = Enum.GetName(typeof(CUE4Parse.UE4.Assets.Exports.Texture.EPixelFormat), cue4parseTexture2D.Format);

            if (cue4parseTexture2D.Mips.Length > 0)
            {
                texture2D.TextureWidth = cue4parseTexture2D.Mips[0].SizeX;
                texture2D.TextureHeight = cue4parseTexture2D.Mips[0].SizeY;
                texture2D.TextureDepth = cue4parseTexture2D.Mips[0].SizeZ;

                if ("TextureCube".Equals(cue4parseTexture2D.Class?.Name))
                {
                    texture2D.NumSlices = 6;
                }
                else if ("Texture2DArray".Equals(cue4parseTexture2D.Class?.Name))
                {
                    texture2D.NumSlices = cue4parseTexture2D.Mips[0].SizeZ;
                }
                
                // TODO: Compute hash
                //texture2D.SourceImageHash = 
            }

            return texture2D;
        }
    }
}
