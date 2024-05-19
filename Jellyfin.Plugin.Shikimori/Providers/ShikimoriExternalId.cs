using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Shikimori.Providers
{
    public class ShikimoriExternalId : IExternalId
    {
        public string ProviderName => ShikimoriPlugin.ProviderName;

        public string Key => ShikimoriPlugin.ProviderId;

        public ExternalIdMediaType? Type
            => ExternalIdMediaType.Series;

        public string? UrlFormatString => ShikimoriPlugin.ShikimoriBaseUrl + "/animes/{0}";

        public bool Supports(IHasProviderIds item)
            => item is Series;

    }
}
