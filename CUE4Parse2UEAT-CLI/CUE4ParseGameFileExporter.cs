using CUE4Parse.UE4.Assets;
using CUE4Parse_Conversion.Textures;
using CUE4Parse2UEAT.Generation;
using SkiaSharp;

namespace CUE4Parse2UEAT.CLI
{
    public class CUE4ParseGameFileExporter : IGameFileExporter<CUE4ParseGameFile>
    {
        protected CUE4ParseGameFileProvider _provider;
        protected string _exportDirectory = string.Empty;

        public CUE4ParseGameFileExporter(CUE4ParseGameFileProvider gameFileProvider)
        {
            _provider = gameFileProvider;
        }

        public void Initialize(ExporterConfig config)
        {
            _exportDirectory = config.ExportDirectory;
        }

        public bool Export(IGameFile gameFile)
        {
            if (gameFile != null && gameFile is not CUE4ParseGameFile)
            {
                throw new ArgumentException($"Not of type \"{nameof(CUE4ParseGameFile)}\"", nameof(gameFile));
            }

            return Export((CUE4ParseGameFile)gameFile);
        }

        public bool Export(CUE4ParseGameFile gameFile)
        {
            if (gameFile == null)
            {
                return false;
            }

            var package = _provider.LoadPackage(gameFile);
            var exports = package?.GetExports();

            if (exports == null)
            {
                return false;
            }

            var uasset = UAssetUtils.CreateUAsset(package as IoPackage);
            var packagePath = Path.Combine(package.Name.Split('/'));
            string json = uasset.Serialize();
            string exportFilePath = Path.Combine(_exportDirectory, Path.ChangeExtension(packagePath, ".json"));
            string exportDirectoryPath = Path.GetDirectoryName(exportFilePath);

            Directory.CreateDirectory(exportDirectoryPath);
            File.WriteAllText(exportFilePath, json);

            foreach (var export in exports)
            {
                ExportRaw(export, exportFilePath);
            }

            return true;
        }

        protected bool ExportRaw(CUE4Parse.UE4.Assets.Exports.UObject export, string filePath)
        {
            if (export == null)
            {
                return false;
            }

            switch (export)
            {
                //case USolarisDigest solarisDigest when isNone:
                //{
                //    if (!TabControl.CanAddTabs)
                //        return false;

                //    TabControl.AddTab($"{solarisDigest.ProjectName}.verse");
                //    TabControl.SelectedTab.Highlighter = AvalonExtensions.HighlighterSelector("verse");
                //    TabControl.SelectedTab.SetDocumentText(solarisDigest.ReadableCode, false);
                //    return true;
                //}
                case CUE4Parse.UE4.Assets.Exports.Texture.UTexture2D texture:
                {
                    var bitmap = texture.Decode(texture.GetFirstMip());

                    if (bitmap == null)
                    {
                        return false;
                    }

                    SaveImage(bitmap, Path.ChangeExtension(filePath, ".png"));
                    return true;
                }
                //case UAkMediaAssetData when isNone:
                //case USoundWave when isNone:
                //{
                //    var shouldDecompress = UserSettings.Default.CompressedAudioMode == ECompressedAudio.PlayDecompressed;
                //    export.Decode(shouldDecompress, out var audioFormat, out var data);
                //    if (data == null || string.IsNullOrEmpty(audioFormat) || export.Owner == null)
                //        return false;

                //    SaveAndPlaySound(Path.Combine(TabControl.SelectedTab.Directory, TabControl.SelectedTab.Header.SubstringBeforeLast('.')).Replace('\\', '/'), audioFormat, data);
                //    return false;
                //}
                //case UWorld when isNone && UserSettings.Default.PreviewWorlds:
                //case UStaticMesh when isNone && UserSettings.Default.PreviewStaticMeshes:
                //case USkeletalMesh when isNone && UserSettings.Default.PreviewSkeletalMeshes:
                //case UMaterialInstance when isNone && UserSettings.Default.PreviewMaterials && !ModelIsOverwritingMaterial &&
                //                            !(Game == FGame.FortniteGame && export.Owner != null && (export.Owner.Name.EndsWith($"/MI_OfferImages/{export.Name}", StringComparison.OrdinalIgnoreCase) ||
                //                                export.Owner.Name.EndsWith($"/RenderSwitch_Materials/{export.Name}", StringComparison.OrdinalIgnoreCase) ||
                //                                export.Owner.Name.EndsWith($"/MI_BPTile/{export.Name}", StringComparison.OrdinalIgnoreCase))):
                //{
                //    if (SnooperViewer.TryLoadExport(cancellationToken, export))
                //        SnooperViewer.Run();
                //    return true;
                //}
                //case UMaterialInstance m when isNone && ModelIsOverwritingMaterial:
                //{
                //    SnooperViewer.Renderer.Swap(m);
                //    SnooperViewer.Run();
                //    return true;
                //}
                //case UAnimSequence when isNone && ModelIsWaitingAnimation:
                //case UAnimMontage when isNone && ModelIsWaitingAnimation:
                //case UAnimComposite when isNone && ModelIsWaitingAnimation:
                //{
                //    SnooperViewer.Renderer.Animate(export);
                //    SnooperViewer.Run();
                //    return true;
                //}
                //case UStaticMesh when HasFlag(bulk, EBulkType.Meshes):
                //case USkeletalMesh when HasFlag(bulk, EBulkType.Meshes):
                //case USkeleton when UserSettings.Default.SaveSkeletonAsMesh && HasFlag(bulk, EBulkType.Meshes):
                //// case UMaterialInstance when HasFlag(bulk, EBulkType.Materials): // read the fucking json
                //case UAnimSequence when HasFlag(bulk, EBulkType.Animations):
                //{
                //    SaveExport(export, HasFlag(bulk, EBulkType.Auto));
                //    return true;
                //}
                //default:
                //{
                //    if (!loadTextures)
                //        return false;

                //    using var package = new CreatorPackage(export, UserSettings.Default.CosmeticStyle);
                //    if (!package.TryConstructCreator(out var creator))
                //        return false;

                //    creator.ParseForInfo();
                //    TabControl.SelectedTab.AddImage(export.Name, false, creator.Draw(), HasFlag(bulk, EBulkType.Auto));
                //    return true;
                //}
            }

            return false;
        }

        protected void SaveImage(SKBitmap bitmap, string filePath)
        {
            byte[] buffer;
            using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream(buffer = data.ToArray(), false);
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            fs.Write(buffer, 0, buffer.Length);
        }
    }
}
