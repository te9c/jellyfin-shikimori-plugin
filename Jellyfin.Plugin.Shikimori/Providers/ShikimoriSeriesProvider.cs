using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Shikimori.Providers;

public class ShikimoriSeriesProvider : IRemoteMetadataProvider<Series, SeriesInfo>
{
    private readonly ILogger<ShikimoriSeriesProvider> _log;
    private readonly ShikimoriClientManager _shikimoriClientManager;
    public string Name { get; } = ShikimoriPlugin.ProviderName;


    public ShikimoriSeriesProvider(ILogger<ShikimoriSeriesProvider> logger, ShikimoriClientManager shikimoriClientManager)
    {
        _log = logger;
        _shikimoriClientManager = shikimoriClientManager;
    }

    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo, CancellationToken cancellationToken)
    {
        _log.LogDebug($"Searching metadata for {searchInfo.Name}");

        var result = new List<RemoteSearchResult>();

        var aid = searchInfo.ProviderIds.GetValueOrDefault(ShikimoriPlugin.ProviderId);
        long id;

        if (!String.IsNullOrEmpty(aid) && long.TryParse(aid, out id))
        {
            var aidResult = await _shikimoriClientManager.GetAnimeAsync(id);
            if (aidResult != null)
            {
                result.Add(aidResult.ToSearchResult());
            }
        }

        if (!String.IsNullOrEmpty(searchInfo.Name))
        {
            var searchResult = await _shikimoriClientManager.SearchAnimeAsync(searchInfo.Name);
            result.AddRange(searchResult);
        }

        return result;
    }

    public Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
