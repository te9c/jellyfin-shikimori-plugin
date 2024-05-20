using Jellyfin.Plugin.Shikimori.Configuration;
using MediaBrowser.Model.Providers;
using ShikimoriSharp.Bases;

namespace Jellyfin.Plugin.Shikimori
{
    public static class AnimeMangaBaseExtensions
    {
        public static RemoteSearchResult ToSearchResult(this AnimeMangaBase anime)
        {
            var result = new RemoteSearchResult();
            result.Name = ShikimoriPlugin.Instance?.Configuration.TitlePreference == TitlePreferenceType.Romaji ? anime.Name : anime.Russian;
            result.ProductionYear = anime.AiredOn?.DateTime.Year;
            result.ProviderIds = new Dictionary<string, string>() { { ShikimoriPlugin.ProviderId, anime.Id.ToString() } };
            result.ImageUrl = ShikimoriPlugin.ShikimoriBaseUrl + anime.Image.Original;
            result.SearchProviderName = ShikimoriPlugin.ProviderName;


            return result;
        }
    }
}
