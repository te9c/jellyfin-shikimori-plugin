using Jellyfin.Plugin.Shikimori.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaBrowser.Controller.Entities.TV;

namespace Jellyfin.Plugin.Shikimori.Providers
{
    public class ShikimoriImageProvider : IRemoteImageProvider
    {
        private ShikimoriClientManager _shikimoriClientManager;
        public string Name { get; } = "Shikimori";

        public ShikimoriImageProvider(ShikimoriClientManager shikimoriClientManager)
        {
            _shikimoriClientManager = shikimoriClientManager;
        }

        public bool Supports(BaseItem item)
        {
            return item is Series;
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            var aid = item.ProviderIds.GetValueOrDefault(ShikimoriPlugin.ProviderId);
            long id;

            var result = new List<RemoteImageInfo>();

            if (aid == null || !long.TryParse(aid, out id))
            {
                return result;
            }

            var anime = await _shikimoriClientManager.GetAnimeAsync(id);
            if (anime?.poster?.originalUrl != null)
            {
                RemoteImageInfo primary = new()
                {
                    ProviderName = Name,
                    Type = ImageType.Primary,
                    Url = ShikimoriPlugin.ShikimoriBaseUrl + anime.poster.originalUrl,
                };
                result.Add(primary);
            }

            return result;
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            var httpClient = ShikimoriPlugin.Instance!.HttpClientFactory.CreateClient();

            return await httpClient.GetAsync(url, cancellationToken);
        }
    }
}
