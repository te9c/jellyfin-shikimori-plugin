using Jellyfin.Plugin.Shikimori.Api;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

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

        public readonly string[] MovieKinds = { "movie" };
        public readonly string[] TvKinds = { "tv", "ona" };

        private ShikimoriApi _shikimoriClient;
        private ILogger _logger;

        public ShikimoriClientManager(ILogger<ShikimoriClientManager> logger)
        {
            _shikimoriClient = new ShikimoriApi(_clientName);

            _logger = logger;
        }

        public async Task<List<RemoteSearchResult>> SearchAnimesAsync(AnimeType type, string name, int? year = null)
        {
            var searchResult = (await _shikimoriClient.GetAnimesAsync(new SearchOptions
            {
                search = name,
                limit = ShikimoriPlugin.Instance!.Configuration.SearchLimit,
                kind = type switch
                {
                    AnimeType.Movie => string.Join(',', MovieKinds),
                    AnimeType.Tv => string.Join(',', TvKinds),
                    _ => null
                },
            })).ToList();

            var result = new List<RemoteSearchResult>();
            foreach (var anime in searchResult.Where(i =>
            {
                if (year.HasValue && i.airedOn != null && i.airedOn.year.HasValue)
                {
                    // One year tolerance
                    return Math.Abs(year.Value - i.airedOn.year.Value) <= 1;
                }

                return true;
            }))
            {
                result.Add(anime.ToSearchResult());
            }

            return result;
        }

        public async Task<Anime?> GetAnimeAsync(long id, AnimeType? type = null)
        {
            var anime = await _shikimoriClient.GetAnimeAsync(id);
            if (anime == null) return anime;

            return type switch
            {
                AnimeType.Tv => TvKinds.Contains(anime.kind) ? anime : null,
                AnimeType.Movie => MovieKinds.Contains(anime.kind) ? anime : null,
                _ => anime,
            };
        }

        public async Task<Anime?> GetAnimeAsync(AnimeType type, string name)
        {
            var searchResult = await _shikimoriClient.GetAnimesAsync(new SearchOptions
            {
                search = name,
                limit = 1,
                kind = type switch
                {
                    AnimeType.Movie => string.Join(',', MovieKinds),
                    AnimeType.Tv => string.Join(',', TvKinds),
                    _ => null
                },
            });

            return searchResult.FirstOrDefault();
        }
    }
}
