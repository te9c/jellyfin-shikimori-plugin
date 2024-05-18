using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Shikimori.Providers;

public class ShikimoriSeriesProvider : IRemoteMetadataProvider<Series, SeriesInfo>
{
    private readonly ILogger<ShikimoriSeriesProvider> _log;
    private readonly ShikimoriClientManager _shikimoriClientManager;
    public string Name { get; } = "Shikimori";


    public ShikimoriSeriesProvider(ILogger<ShikimoriSeriesProvider> logger, ShikimoriClientManager shikimoriClientManager)
    {
        _log = logger;
        _shikimoriClientManager = shikimoriClientManager;
    }

    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
