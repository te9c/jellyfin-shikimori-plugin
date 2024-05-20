using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using ShikimoriSharp;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;

namespace Jellyfin.Plugin.Shikimori
{
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

        public async Task<List<RemoteSearchResult>> SearchAnimeSeriesAsync(string name, int? year = null)
        {
            var searchResult = await _shikimoriClient.Animes.GetAnime(new AnimeRequestSettings
            {
                search = name,
                limit = ShikimoriPlugin.Instance?.Configuration.SearchLimit,
                kind = "tv",
            });

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

        public async Task<AnimeMangaBase?> GetAnimeAsync(long id)
        {
            return await _shikimoriClient.Animes.GetAnime(id);
        }
    }
}
