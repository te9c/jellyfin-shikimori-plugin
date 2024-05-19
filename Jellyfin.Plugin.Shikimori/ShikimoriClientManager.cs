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
        private ShikimoriClient _shikimoriClient;
        private ILogger _logger;

        public ShikimoriClientManager(ILogger<ShikimoriClientManager> logger)
        {
            // TODO: Think about logger.
            // Maybe it's not a good idea to pass main logger

            // TODO: Maybe move constants like name and client id to other file?

            _shikimoriClient = new ShikimoriClient(logger, new ClientSettings(
                        "Shikimori Jellyfin plugin",
                        "4jL1c_MSgZ4qjC8yNotwYGXQmhJ9wFCukQMm_48vGCY",
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

    public static class AnimeMangaBaseExtensions
    {
        public static RemoteSearchResult ToSearchResult(this AnimeMangaBase anime)
        {
            var result = new RemoteSearchResult();
            result.Name = anime.Name;
            result.ProductionYear = anime.ReleasedOn?.DateTime.Year;
            result.ProviderIds = new Dictionary<string, string>() { { "Shikimori", anime.Id.ToString() } };
            result.ImageUrl = "https://shikimori.one" + anime.Image.Original;
            result.SearchProviderName = "Shikimori";


            return result;
        }
    }
}
