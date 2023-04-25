namespace CUE4Parse2UEAT.CLI
{
    public interface IGameFileExporter
    {
        void Initialize(ExporterConfig config);
        bool Export(IGameFile gameFile);
    }

    public interface IGameFileExporter<in T> : IGameFileExporter where T : IGameFile
    {
        bool Export(T gameFile);
    }
}
