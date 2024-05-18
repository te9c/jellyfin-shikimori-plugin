using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Shikimori.Providers
{
    public class ShikimoriExternalId : IExternalId
    {
        public string ProviderName => "Shikimori";

        public string Key => "Shikimori";

        public ExternalIdMediaType? Type
            => ExternalIdMediaType.Series;

        public string? UrlFormatString => "https://shikimori.one/animes/{0}";

        public bool Supports(IHasProviderIds item)
            => item is Series;

    }
}
