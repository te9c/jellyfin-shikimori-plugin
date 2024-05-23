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

        private static string? GetPreferedTitle(TitlePreferenceType type, AnimeID anime)
        {
            // TODO: Fallback languages?
            return type switch
            {
                TitlePreferenceType.Japanese => anime.Japanese.FirstOrDefault(),
                TitlePreferenceType.Romaji => anime.Name,
                TitlePreferenceType.Russian => anime.Russian,
                _ => null
            };
        }

        private static string? GetPreferedGenreTitle(GenreTitleLanguagePreferenceType type, Genre genre)
        {
            return type switch
            {
                GenreTitleLanguagePreferenceType.English => genre.Name,
                GenreTitleLanguagePreferenceType.Russian => genre.Russian,
                _ => null
            };
        }
        public static Series ToSeries(this AnimeID anime)
        {
            var result = new Series
            {
                Name = GetPreferedTitle(ShikimoriPlugin.Instance!.Configuration.TitlePreference, anime),
                OriginalTitle = GetPreferedTitle(ShikimoriPlugin.Instance!.Configuration.OriginalTitlePreference, anime),
                Overview = anime.DescriptionHtml,
                ProductionYear = anime.AiredOn?.Year,
                PremiereDate = anime.AiredOn?.DateTime,
                EndDate = anime.ReleasedOn?.DateTime,
                Genres = anime.Genres.Select(i => GetPreferedGenreTitle(ShikimoriPlugin.Instance!.Configuration.GenreTitleLanguagePreference, i)).ToArray(),
                CommunityRating = float.Parse(anime.Score),
                OfficialRating = FormatRating(anime.Rating),
                Studios = anime.Studios.Select(i => { return i.Name;}).ToArray(),
                Status = anime.Status switch
                {
                    "released" => SeriesStatus.Ended,
                    "ongoing" => SeriesStatus.Continuing,
                    "anons" => SeriesStatus.Unreleased,
                    _ => null,
                },
                ProviderIds = new Dictionary<string, string> {{ShikimoriPlugin.ProviderName, anime.Id.ToString()}},
            };

            return result;
        }
    }
}
