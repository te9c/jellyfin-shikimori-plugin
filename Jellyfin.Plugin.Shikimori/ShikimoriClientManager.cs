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

        public async Task<List<RemoteSearchResult>> SearchAnimeAsync(string name)
        {
            var searchResult = await _shikimoriClient.Animes.GetAnime(new AnimeRequestSettings
            {
                search = name
            });

            var result = new List<RemoteSearchResult>();
            foreach (var anime in searchResult)
            {
                result.Add(anime.ToSearchResult());
            }

            return result;
        }

        public async Task<AnimeMangaBase?> GetAnimeAsync(long id)
        {
            var result = await _shikimoriClient.Animes.GetAnime(id);

            return result;
        }
    }
}
