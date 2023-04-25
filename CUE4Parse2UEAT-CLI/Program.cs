namespace CUE4Parse2UEAT.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (!ValidateArgs(args, out string gameDirectory, out string exportDirectory, out string assetPackagePath))
                {
                    WriteHelp();
                    return;
                }

                // CUE4ParseGameFileProvider is hard-coded for Hogwarts Legacy
                var provider = new CUE4ParseGameFileProvider(gameDirectory);
                var exporter = new CUE4ParseGameFileExporter(provider);
                exporter.Initialize(new ExporterConfig.Builder().SetExportDirectory(exportDirectory).Build());

                var gameFile = provider.GetGameFile(assetPackagePath);
                exporter.Export(gameFile);

                Console.WriteLine($"Completed export of \"{gameFile?.Path}\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected exception: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.Out.WriteLine();
            Console.Out.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static bool ValidateArgs(string[] args, out string gameDirectory, out string exportDirectory, out string assetPackagePath)
        {
            gameDirectory = string.Empty;
            exportDirectory = string.Empty;
            assetPackagePath = string.Empty;

            if (args.Length < 3)
            {
                Console.Out.WriteLine($"Error: Expecting 3 args; only received {args.Length}");
                Console.Out.WriteLine();
                return false;
            }

            if (args.Length > 3)
            {
                Console.Out.WriteLine($"Error: Expecting 3 args but received {args.Length}; ignore the extras...");
                Console.Out.WriteLine();
            }

            try
            {
                gameDirectory = Path.GetFullPath(args[0]);

                if (!Directory.Exists(gameDirectory))
                {
                    Console.Out.WriteLine($"Error: Game directory \"{gameDirectory}\" does not exist");
                    Console.Out.WriteLine();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine($"Error: Game directory path is not valid: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.Out.WriteLine();
                return false;
            }

            try
            {
                exportDirectory = Path.GetFullPath(args[1]);

                if (!Directory.Exists(exportDirectory))
                {
                    Console.Out.WriteLine($"Export directory \"{exportDirectory}\" does not exist; creating it...");
                    Console.Out.WriteLine();
                    Directory.CreateDirectory(exportDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine($"Error: Export directory path is not valid: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.Out.WriteLine();
                return false;
            }

            assetPackagePath = args[2];

            return true;
        }

        static void WriteHelp()
        {
            Console.Out.WriteLine("Usage:");
            Console.Out.WriteLine("\tCUE4Parse2UEAT-CLI.exe gameDir exportDir assetPackagePath");
            Console.Out.WriteLine();
            Console.Out.WriteLine("Example:");
            Console.Out.WriteLine("\tCUE4Parse2UEAT-CLI.exe \"C:\\Steam\\SteamApps\\common\\Hogwarts Legacy\\Phoenix\" \"C:\\Dump\" \"Phoenix/Content/Pawn/Player/BP_Biped_Player.uasset\"");
            Console.Out.WriteLine();
            Console.Out.WriteLine("\tgameDir\t\t\tGame content directory.");
            Console.Out.WriteLine("\texportDir\t\tDirectory where asset json is exported to.");
            Console.Out.WriteLine("\tassetPackagePath\tPackage path of asset to export.");
            Console.Out.WriteLine();
        }
    }
}