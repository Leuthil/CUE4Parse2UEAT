namespace CUE4Parse2UEAT
{
    public interface IGameFile
    {
        string Name { get; }
        string Path { get; }
    }

    public interface IGameFileProvider
    {
        IGameFile? GetGameFile(string path);
        IEnumerable<IGameFile> GetGameFiles();
    }

    public interface IGameFileProvider<out T> : IGameFileProvider where T : IGameFile
    {
        new T? GetGameFile(string path);
        new IEnumerable<T> GetGameFiles();
    }
}
