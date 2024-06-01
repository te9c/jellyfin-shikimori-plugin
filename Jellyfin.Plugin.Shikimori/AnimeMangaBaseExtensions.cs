using Jellyfin.Plugin.Shikimori.Configuration;
using MediaBrowser.Model.Providers;
using ShikimoriSharp.Bases;

namespace Jellyfin.Plugin.Shikimori
{
    public static class AnimeMangaBaseExtensions
    {
        private static string? GetPreferedTitle(SearchTitlePreferenceType type, AnimeMangaBase anime)
        {
            return type switch
            {
                SearchTitlePreferenceType.Romaji => anime.Name,
                SearchTitlePreferenceType.Russian => anime.Russian,
                _ => null,
            };
        }

        public static RemoteSearchResult ToSearchResult(this AnimeMangaBase anime)
        {
            var result = new RemoteSearchResult()
            {
                Name = GetPreferedTitle(ShikimoriPlugin.Instance!.Configuration.SearchTitlePreference, anime),
                ProductionYear = anime.AiredOn?.DateTime.Year,
                PremiereDate = anime.AiredOn?.DateTime,
                ProviderIds = new Dictionary<string, string>() { { ShikimoriPlugin.ProviderId, anime.Id.ToString() } },
                ImageUrl = ShikimoriPlugin.ShikimoriBaseUrl + anime.Image.Original,
                SearchProviderName = ShikimoriPlugin.ProviderName
            };

            return result;
        }
    }
}
