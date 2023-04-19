using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse2UEAT
{
    public class CUE4ParseGameFileProvider : IGameFileProvider<CUE4ParseGameFile>
    {
        public IFileProvider Provider { get; protected set; }

        public CUE4ParseGameFileProvider(string gameFileDirectory)
        {
            // TODO: Make this not hard-coded for Hogwarts Legacy
            var overriddenMapStructTypes = new Dictionary<string, KeyValuePair<string, string>>
            {
                ["GaitTags"] = new("", "GameplayTagContainer"),
                ["DefaultDirectionInComponentSpace"] = new("", "Vector"),
                ["NamedWorldRegions"] = new("", "Box"),
                ["AcousticTextureParamsMap"] = new("Guid", ""),
                ["FieldCache"] = new("Guid", ""),
                ["VectorOverrides"] = new("", "LinearColor"),
                ["VarMapKeyedVector2Ds"] = new("", "Vector2D"),
                ["VarMapKeyedVectors"] = new("", "Vector"),
                ["LabelToIdCache"] = new("", "Guid"),
                ["RiverFlowPoints"] = new("Vector", "Vector2D"),
                ["NameToGuidMap"] = new("", "Guid"),
                ["StringColorMap"] = new("", "Color"),
                ["EventTracks"] = new("", "CacheEventTrack"),
                ["BoundActorComponents"] = new("Guid", ""),
                ["ActiveAbilities"] = new("Guid", ""),
                ["PointsOfInterest"] = new("", "Vector"),
                ["VectorParameterValues"] = new("", "LinearColor"),
                ["GaitToTagsMap"] = new("", "GameplayTagContainer"),
                ["BroomParams"] = new("", "Vector2D"),
                ["MatParams"] = new("", "Vector2D"),
                ["SoftwareCursors"] = new("", "SoftClassPath"),
                ["SubSectionRanges"] = new("Guid", "MovieSceneFrameRange"),
                ["PendingLoads"] = new("SoftObjectPath", ""),
                ["LinearColors"] = new("", "LinearColor"),
                ["Rotators"] = new("", "Rotator"),
                ["StrToVec"] = new("", "Vector"),
                ["Broom_Params_39_03967392492EB7A85F6B458CD76B0E26"] = new("", "Vector2D"),
                ["Material_Params_44_A64816404EFD13C4FA4B3F9AD5244000"] = new("", "Vector2D")
            };

            var versions = new VersionContainer(
                CUE4Parse.UE4.Versions.EGame.GAME_HogwartsLegacy,
                CUE4Parse.UE4.Assets.Exports.Texture.ETexturePlatform.DesktopMobile,
                customVersions: null,
                optionOverrides: null,
                mapStructTypesOverrides: overriddenMapStructTypes
            );

            var cue4ParseProvider = new DefaultFileProvider(gameFileDirectory, SearchOption.AllDirectories, false, versions);
            cue4ParseProvider.Initialize();
            cue4ParseProvider.SubmitKey(new FGuid(), new CUE4Parse.Encryption.Aes.FAesKey("0000000000000000000000000000000000000000000000000000000000000000"));

            Provider = cue4ParseProvider;
        }

        public CUE4ParseGameFile? GetGameFile(string path)
        {
            if (!Provider.TryFindGameFile(path, out var gameFile))
            {
                return null;
            }

            return new CUE4ParseGameFile(gameFile);
        }

        public IEnumerable<CUE4ParseGameFile> GetGameFiles()
        {
            return Provider.Files.Select(gameFile => new CUE4ParseGameFile(gameFile.Value));
        }

        public IPackage LoadPackage(CUE4ParseGameFile gameFile)
        {
            return Provider.LoadPackage(gameFile.GameFile);
        }

        IGameFile? IGameFileProvider.GetGameFile(string path)
        {
            return GetGameFile(path);
        }

        IEnumerable<IGameFile> IGameFileProvider.GetGameFiles()
        {
            return GetGameFiles();
        }
    }
}
