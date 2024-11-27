using Jellyfin.Plugin.Shikimori.Api;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Shikimori.Providers
{
    public class ShikimoriMovieProvider : IRemoteMetadataProvider<Movie, MovieInfo>
    {
        private readonly ILogger<ShikimoriMovieProvider> _log;
        private readonly ShikimoriClientManager _shikimoriClientManager;
        private readonly ProviderIdResolver _providerIdResolver;
        public string Name { get; } = ShikimoriPlugin.ProviderName;

        public ShikimoriMovieProvider(ILogger<ShikimoriMovieProvider> logger,
                                      ShikimoriClientManager shikimoriClientManager,
                                      ProviderIdResolver providerIdResolver)
        {
            _log = logger;
            _shikimoriClientManager = shikimoriClientManager;
            _providerIdResolver = providerIdResolver;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo searchInfo, CancellationToken cancellationToken)
        {
            var result = new List<RemoteSearchResult>();

            long id;
            if (_providerIdResolver.TryResolve(searchInfo, out id))
            {
                var aidResult = await _shikimoriClientManager.GetAnimeAsync(id, cancellationToken).ConfigureAwait(false);
                if (aidResult != null)
                {
                    result.Add(aidResult.ToSearchResult());
                }
            }

            if (!String.IsNullOrEmpty(searchInfo.Name))
            {
                var searchResult = await _shikimoriClientManager.SearchAnimesAsync(searchInfo.Name, cancellationToken, AnimeType.Movie, searchInfo.Year).ConfigureAwait(false);
                result.AddRange(searchResult);
            }

            return result;
        }

        // So basically I should parse path that movieinfo have to get
        // attribute like [shiki-XXXXX], where XXXXX is id on shikimroi
        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancellationToken)
        {
            var result = new MetadataResult<Movie>();
            Anime? anime = null;

            long id;
            if (_providerIdResolver.TryResolve(info, out id))
            {
                anime = await _shikimoriClientManager.GetAnimeAsync(id, cancellationToken).ConfigureAwait(false);
                result.QueriedById = true;
            }

            if (anime == null)
            {
                _log.LogDebug($"Searching {info.Name}");
                anime = await _shikimoriClientManager.GetAnimeAsync(info.Name, cancellationToken, AnimeType.Movie).ConfigureAwait(false);
                result.QueriedById = false;
            }

            if (anime != null)
            {
                result.HasMetadata = true;
                result.Item = anime.ToMovie();
                // result.People = anime.GetPeopleInfo();
                result.Provider = ShikimoriPlugin.ProviderName;
                result.ResultLanguage = "ru";
            }

            return result;
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            var httpClient = ShikimoriPlugin.Instance!.HttpClientFactory.CreateClient();

            return await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        }
    }
}
