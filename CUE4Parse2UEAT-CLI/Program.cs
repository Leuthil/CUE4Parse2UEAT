using CUE4Parse2UEAT;

namespace CUE4Parse2UEAT_CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string gameDirectory = @"C:\Games\Steam\SteamApps\common\Hogwarts Legacy\Phoenix";
            string exportDirectory = @"F:\Repos\HogwartsModding\TestDump";

            // CUE4ParseGameFileProvider is hard-coded for Hogwarts Legacy
            var provider = new CUE4ParseGameFileProvider(gameDirectory);
            var exporter = new CUE4ParseGameFileExporter(provider);
            exporter.Initialize(new ExporterConfig.Builder().SetExportDirectory(exportDirectory).Build());

            //var assetFullPath = @"Phoenix/Content/UI/Conjuration/UI_T_ChalkboardSign_Conjuration.uasset";
            //var assetFullPath = @"Phoenix/Content/UI/Conjuration/UI_BP_ConjurationScreen.uasset";
            var assetFullPath = @"Phoenix/Content/Gameplay/BluePrints/Universal/BP_WaveSpawner.uasset";

            var gameFile = provider.GetGameFile(assetFullPath);
            exporter.Export(gameFile);
        }
    }
}