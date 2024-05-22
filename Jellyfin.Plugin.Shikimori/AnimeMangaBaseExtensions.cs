using System.Diagnostics;
using MediaBrowser.Controller.Entities.TV;
using Jellyfin.Plugin.Shikimori.Configuration;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;

namespace Jellyfin.Plugin.Shikimori
{
    public static class AnimeMangaBaseExtensions
    {
        public static RemoteSearchResult ToSearchResult(this AnimeMangaBase anime)
        {
            var result = new RemoteSearchResult();
            result.Name = ShikimoriPlugin.Instance?.Configuration.SearchTitlePreference == SearchTitlePreferenceType.Romaji ? anime.Name : anime.Russian;
            result.ProductionYear = anime.AiredOn?.DateTime.Year;
            result.PremiereDate = anime.AiredOn?.DateTime;
            result.ProviderIds = new Dictionary<string, string>() { { ShikimoriPlugin.ProviderId, anime.Id.ToString() } };
            result.ImageUrl = ShikimoriPlugin.ShikimoriBaseUrl + anime.Image.Original;
            result.SearchProviderName = ShikimoriPlugin.ProviderName;


            return result;
        }
    }

    public static class AnimeIdExtensions
    {
        private static string? FormatRating(string shikimoriRating)
        {
            return shikimoriRating switch
            {
                "g" => "G",
                "pg" => "PG",
                "pg_13" => "PG-13",
                "r" => "R",
                "r_plus" => "R+",
                "rx" => "Rx",
                _ => null
            };
        }
        public static Series ToSeries(this AnimeID anime)
        {
            var result = new Series
            {
                Name = anime.Russian,
                OriginalTitle = anime.Japanese.FirstOrDefault(),
                Overview = anime.DescriptionHtml,
                ProductionYear = anime.AiredOn?.Year,
                PremiereDate = anime.AiredOn?.DateTime,
                EndDate = anime.ReleasedOn?.DateTime,
                Genres = anime.Genres.Select(i => i.Russian).ToArray(),
                CommunityRating = float.Parse(anime.Score),
                OfficialRating = FormatRating(anime.Rating),
                Studios = anime.Studios.Select(i => { return i.Name;}).ToArray(),
                ProviderIds = new Dictionary<string, string> {{ShikimoriPlugin.ProviderName, anime.Id.ToString()}},
            };
            if (anime.Status == "released")
            {
                result.Status = SeriesStatus.Ended;
            }
            else if (anime.Status == "ongoing")
            {
                result.Status = SeriesStatus.Continuing;
            }
            else if (anime.Status == "anons")
            {
                result.Status = SeriesStatus.Unreleased;
            }

            return result;
        }
    }
}
