using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Classes;

namespace Jellyfin.Plugin.Shikimori.Providers;

public class ShikimoriMovieProvider : IRemoteMetadataProvider<Movie, MovieInfo>
{
    private readonly ILogger<ShikimoriMovieProvider> _log;
    private readonly ShikimoriClientManager _shikimoriClientManager;
    public string Name { get; } = ShikimoriPlugin.ProviderName;

    public ShikimoriMovieProvider(ILogger<ShikimoriMovieProvider> logger, ShikimoriClientManager shikimoriClientManager)
    {
        _log = logger;
        _shikimoriClientManager = shikimoriClientManager;
    }

    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo searchInfo, CancellationToken cancellationToken)
    {
        _log.LogDebug($"Searching metadata for {searchInfo.Name}");

        var result = new List<RemoteSearchResult>();

        var aid = searchInfo.ProviderIds.GetValueOrDefault(ShikimoriPlugin.ProviderId);
        long id;

        if (!String.IsNullOrEmpty(aid) && long.TryParse(aid, out id)) {
            var aidResult = await _shikimoriClientManager.GetAnimeAsync(id);
            if (aidResult != null) {
                // TODO: Check for kind
                result.Add(aidResult.ToSearchResult());
            }
        }

        if (!String.IsNullOrEmpty(searchInfo.Name)) {
            var searchResult = await _shikimoriClientManager.SearchAnimeSeriesAsync(AnimeType.Movie, searchInfo.Name, searchInfo.Year);
            result.AddRange(searchResult);
        }

        return result;
    }

    public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancellationToken)
    {
        var result = new MetadataResult<Movie>();
        AnimeID? anime = null;
        var aid = info.ProviderIds.GetValueOrDefault(ShikimoriPlugin.ProviderId);
        long id;
        if (!String.IsNullOrEmpty(aid) && long.TryParse(aid, out id)) {
            anime = await _shikimoriClientManager.GetAnimeAsync(id);
            result.QueriedById = true;
        } else {
            _log.LogDebug($"Searching {info.Name}");
            anime = await _shikimoriClientManager.GetAnimeAsync(AnimeType.Movie, info.Name);
            result.QueriedById = false;
        }

        if (anime != null) {
            result.HasMetadata = true;
            result.Item = anime.ToMovie();
            result.Provider = ShikimoriPlugin.ProviderName;
        }

        return result;
    }

    public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        var httpClient = ShikimoriPlugin.Instance!.HttpClientFactory.CreateClient();

        return await httpClient.GetAsync(url, cancellationToken);
    }
}