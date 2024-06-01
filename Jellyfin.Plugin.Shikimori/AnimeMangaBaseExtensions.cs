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
            var result = new RemoteSearchResult();
            result.Name = GetPreferedTitle(ShikimoriPlugin.Instance!.Configuration.SearchTitlePreference, anime);
            result.ProductionYear = anime.AiredOn?.DateTime.Year;
            result.PremiereDate = anime.AiredOn?.DateTime;
            result.ProviderIds = new Dictionary<string, string>() { { ShikimoriPlugin.ProviderId, anime.Id.ToString() } };
            result.ImageUrl = ShikimoriPlugin.ShikimoriBaseUrl + anime.Image.Original;
            result.SearchProviderName = ShikimoriPlugin.ProviderName;


            return result;
        }
    }
}
