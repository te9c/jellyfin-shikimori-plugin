using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using ShikimoriSharp;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;

namespace Jellyfin.Plugin.Shikimori
{
    public enum AnimeType
    {
        Tv,
        Movie
    }
    public class ShikimoriClientManager
    {
        private const string _clientName = "Shikimori Jellyfin plugin";
        private const string _clientId = "4jL1c_MSgZ4qjC8yNotwYGXQmhJ9wFCukQMm_48vGCY";

        private readonly string[] _movieKinds = { "movie" };
        // I actually dont know what is tv_13, tv_24 and tv_48
        // But shikimori's api provide this kinds, so I include them
        private readonly string[] _tvKinds = { "tv", "ona", "tv_13", "tv_24", "tv_48" };

        private ShikimoriClient _shikimoriClient;
        private ILogger _logger;

        public ShikimoriClientManager(ILogger<ShikimoriClientManager> logger)
        {
            _shikimoriClient = new ShikimoriClient(logger, new ClientSettings(
                        _clientName,
                        _clientId,
                        ""
                        ));

            _logger = logger;
        }

        public async Task<List<RemoteSearchResult>> SearchAnimeSeriesAsync(AnimeType type, string name, int? year = null)
        {
            var searchResult = (await _shikimoriClient.Animes.GetAnime(new AnimeRequestSettings
            {
                search = name,
                limit = ShikimoriPlugin.Instance?.Configuration.SearchLimit,
                kind = type switch
                {
                    AnimeType.Movie => string.Join(',', _movieKinds),
                    AnimeType.Tv => string.Join(',', _tvKinds),
                    _ => null
                },
            })).ToList();

            var result = new List<RemoteSearchResult>();
            foreach (var anime in searchResult.Where(i =>
            {
                if (year.HasValue && i.AiredOn.HasValue)
                {
                    // One year tolerance
                    return Math.Abs(year.Value - i.AiredOn.Value.Year) <= 1;
                }

                return true;
            }))
            {
                result.Add(anime.ToSearchResult());
            }

            return result;
        }

        public async Task<AnimeID?> GetAnimeAsync(long id, AnimeType? type = null)
        {
            var anime = await _shikimoriClient.Animes.GetAnime(id);
            if (anime == null) return anime;

            return type switch
            {
                AnimeType.Tv => _tvKinds.Contains(anime.Kind) ? anime : null,
                AnimeType.Movie => _movieKinds.Contains(anime.Kind) ? anime : null,
                _ => anime,
            };
        }

        public async Task<AnimeID?> GetAnimeAsync(AnimeType type, string name)
        {
            var searchResult = await _shikimoriClient.Animes.GetAnime(new AnimeRequestSettings
            {
                search = name,
                limit = 1,
                kind = type switch
                {
                    AnimeType.Movie => string.Join(',', _movieKinds),
                    AnimeType.Tv => string.Join(',', _tvKinds),
                    _ => null
                },
            });

            AnimeID? result = null;
            var id = searchResult.FirstOrDefault()?.Id;
            if (id is not null)
            {
                result = await GetAnimeAsync(id.Value);
            }
            return result;
        }
    }
}
