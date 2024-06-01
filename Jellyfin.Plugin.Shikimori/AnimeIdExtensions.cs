using MediaBrowser.Controller.Entities.TV;
using Jellyfin.Plugin.Shikimori.Configuration;
using MediaBrowser.Model.Entities;
using ShikimoriSharp.Classes;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities;

namespace Jellyfin.Plugin.Shikimori
{
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

        private static string? GetPreferedGenreTitle(GenreTitleLanguagePreferenceType type, ShikimoriSharp.Classes.Genre genre)
        {
            return type switch
            {
                GenreTitleLanguagePreferenceType.English => genre.Name,
                GenreTitleLanguagePreferenceType.Russian => genre.Russian,
                _ => null
            };
        }

        private static void FillBaseItem(BaseItem item, AnimeID anime)
        {
            PluginConfiguration config = ShikimoriPlugin.Instance!.Configuration;

            item.Name = GetPreferedTitle(config.TitlePreference, anime);
            item.OriginalTitle = GetPreferedTitle(config.OriginalTitlePreference, anime);
            item.Overview = anime.DescriptionHtml;
            item.ProductionYear = anime.AiredOn?.Year;
            item.PremiereDate = anime.AiredOn?.DateTime;
            item.EndDate = anime.ReleasedOn?.DateTime;
            item.Genres = anime.Genres.Select(i => GetPreferedGenreTitle(config.GenreTitleLanguagePreference, i)).ToArray();
            item.CommunityRating = float.Parse(anime.Score);
            item.OfficialRating = FormatRating(anime.Rating);
            item.Studios = anime.Studios.Select(i => { return i.Name; }).ToArray();
            item.ProviderIds = new Dictionary<string, string> { { ShikimoriPlugin.ProviderName, anime.Id.ToString() } };
        }

        public static Movie ToMovie(this AnimeID anime)
        {
            var result = new Movie();
            FillBaseItem(result, anime);

            return result;
        }

        public static Series ToSeries(this AnimeID anime)
        {
            var result = new Series()
            {
                Status = anime.Status switch
                {
                    "released" => SeriesStatus.Ended,
                    "ongoing" => SeriesStatus.Continuing,
                    "anons" => SeriesStatus.Unreleased,
                    _ => null,
                },
            };
            FillBaseItem(result, anime);

            return result;
        }
    }
}
