using UEATSerializer.Serializer;
using Newtonsoft.Json;

namespace UEATSerializer.UEAT
{
    public class UTexture2D : UObject
    {
        /// <summary>
        /// Serialization disabled. No need to fill out.
        /// </summary>
        //public string LightingGuid { get; set; }

        /// <summary>
        /// Serialization disabled. No need to fill out.
        /// </summary>
        //public string ImportedSize { get; set; }

        /// <summary>
        /// Serialization disabled. No need to fill out.
        /// Not sure what the property actually is, should be int32 of some sort)
        /// </summary>
        //public string FirstResourceMemMip { get; set; }

        public int TextureWidth { get; set; }
        public int TextureHeight { get; set; }
        public int TextureDepth { get; set; }
        public int NumSlices { get; set; }

        /// <summary>
        /// UTexture2D::GetPixelFormatEnum() value name as string
        /// </summary>
        public string CookedPixelFormat { get; set; }

        /// <summary>
        /// Used so the Asset Generator can easily figure out whenever a refresh is needed.
        /// </summary>
        public string SourceTextureFileName { get; set; }

        protected override void WriteJsonForData(JsonWriter writer, JsonSerializer serializer, PackageObjectHierarchy objectHierarchy)
        {
            writer.WritePropertyName("TextureWidth");
            writer.WriteValue(TextureWidth);
            writer.WritePropertyName("TextureHeight");
            writer.WriteValue(TextureHeight);
            writer.WritePropertyName("TextureDepth");
            writer.WriteValue(TextureDepth);
            writer.WritePropertyName("NumSlices");
            writer.WriteValue(NumSlices);

            if (CookedPixelFormat != null)
            {
                writer.WritePropertyName("CookedPixelFormat");
                writer.WriteValue(CookedPixelFormat);
            }


            if (SourceTextureFileName != null)
            {
                writer.WritePropertyName("SourceImageHash");
                writer.WriteValue(Hash(SourceTextureFileName));
            }
        }

        public static string Hash(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
