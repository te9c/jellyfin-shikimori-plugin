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
                kind = type switch { 
                    AnimeType.Movie => "movie",
                    AnimeType.Tv => "tv",
                    _ => null 
                },
            })).ToList();
            
            // kostil
            // because in shikimori API tv and ona is different kinds of media, but ona is technically tv series
            if (type == AnimeType.Tv && searchResult.Count() < ShikimoriPlugin.Instance?.Configuration.SearchLimit)
            {
                searchResult.AddRange(await _shikimoriClient.Animes.GetAnime(new AnimeRequestSettings
                {
                    search = name,
                    limit = ShikimoriPlugin.Instance?.Configuration.SearchLimit - searchResult.Count(),
                    kind = "ona",
                }));
            }

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

        public async Task<AnimeID?> GetAnimeAsync(long id)
        {
            return await _shikimoriClient.Animes.GetAnime(id);
        }
        public async Task<AnimeID?> GetAnimeAsync(AnimeType type, string name)
        {
            var searchResult = await _shikimoriClient.Animes.GetAnime(new AnimeRequestSettings
            {
                search = name,
                limit = 1,
                kind = type switch
                {
                    AnimeType.Movie => "movie",
                    AnimeType.Tv => "tv",
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
