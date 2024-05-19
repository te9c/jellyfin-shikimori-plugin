using MediaBrowser.Model.Providers;
using ShikimoriSharp.Bases;

namespace Jellyfin.Plugin.Shikimori
{
    public static class AnimeMangaBaseExtensions
    {
        public static RemoteSearchResult ToSearchResult(this AnimeMangaBase anime)
        {
            var result = new RemoteSearchResult();
            result.Name = anime.Name;
            result.ProductionYear = anime.ReleasedOn?.DateTime.Year;
            result.ProviderIds = new Dictionary<string, string>() { { ShikimoriPlugin.ProviderId, anime.Id.ToString() } };
            result.ImageUrl = ShikimoriPlugin.ShikimoriBaseUrl + anime.Image.Original;
            result.SearchProviderName = ShikimoriPlugin.ProviderName;


            return result;
        }
    }
}
