using Jellyfin.Plugin.Shikimori.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Entities.Movies;

namespace Jellyfin.Plugin.Shikimori.Providers
{
    public class ShikimoriImageProvider : IRemoteImageProvider
    {
        private ShikimoriClientManager _shikimoriClientManager;
        private ProviderIdResolver _providerIdResolver;
        public string Name { get; } = ShikimoriPlugin.ProviderName;

        public ShikimoriImageProvider(ShikimoriClientManager shikimoriClientManager,
                                      ProviderIdResolver providerIdResolver)
        {
            _shikimoriClientManager = shikimoriClientManager;
            _providerIdResolver = providerIdResolver;
        }

        public bool Supports(BaseItem item)
        {
            return item is Series || item is Movie;
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            var result = new List<RemoteImageInfo>();

            long id;
            if (!_providerIdResolver.TryResolve(item, out id))
            {
                return result;
            }

            var anime = await _shikimoriClientManager.GetAnimeAsync(id, cancellationToken).ConfigureAwait(false);
            if (anime?.poster?.originalUrl != null)
            {
                RemoteImageInfo primary = new()
                {
                    ProviderName = Name,
                    Type = ImageType.Primary,
                    Url = anime.poster.originalUrl,
                };
                result.Add(primary);
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
