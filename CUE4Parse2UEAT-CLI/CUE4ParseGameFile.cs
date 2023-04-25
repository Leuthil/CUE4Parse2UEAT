using CUE4Parse.FileProvider;

namespace CUE4Parse2UEAT.CLI
{
    public class CUE4ParseGameFile : IGameFile
    {
        public string Name => _gameFile.Name;
        public string Path => _gameFile.Path;
        public GameFile GameFile => _gameFile;

        protected GameFile _gameFile;

        public CUE4ParseGameFile(GameFile gameFile)
        {
            _gameFile = gameFile;
        }
    }
}
