using MediaBrowser.Controller.Entities.TV;
using Jellyfin.Plugin.Shikimori.Configuration;
using MediaBrowser.Model.Entities;
using ShikimoriSharp.Classes;
using MediaBrowser.Controller.Entities.Movies;

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

        // TODO: Reduce amount of repeating code
        public static Movie ToMovie(this AnimeID anime)
        {
            if (anime.Kind != "movie")
            {
                throw new ArgumentException("AnimeID kind is not movie", "anime");
            }

            PluginConfiguration config = ShikimoriPlugin.Instance!.Configuration;

            var result = new Movie
            {
                Name = GetPreferedTitle(config.TitlePreference, anime),
                OriginalTitle = GetPreferedTitle(config.OriginalTitlePreference, anime),
                Overview = anime.DescriptionHtml,
                ProductionYear = anime.AiredOn?.Year,
                PremiereDate = anime.AiredOn?.DateTime,
                EndDate = anime.ReleasedOn?.DateTime,
                Genres = anime.Genres.Select(i => GetPreferedGenreTitle(config.GenreTitleLanguagePreference, i)).ToArray(),
                CommunityRating = float.Parse(anime.Score),
                OfficialRating = FormatRating(anime.Rating),
                Studios = anime.Studios.Select(i => { return i.Name; }).ToArray(),
                ProviderIds = new Dictionary<string, string> { { ShikimoriPlugin.ProviderName, anime.Id.ToString() } },
            };

            return result;
        }

        public static Series ToSeries(this AnimeID anime)
        {
            if (anime.Kind != "tv" || anime.Kind != "ona")
            {
                throw new ArgumentException("AnimeID kind is not series", "anime");
            }

            PluginConfiguration config = ShikimoriPlugin.Instance!.Configuration;

            var result = new Series
            {
                Name = GetPreferedTitle(config.TitlePreference, anime),
                OriginalTitle = GetPreferedTitle(config.OriginalTitlePreference, anime),
                Overview = anime.DescriptionHtml,
                ProductionYear = anime.AiredOn?.Year,
                PremiereDate = anime.AiredOn?.DateTime,
                EndDate = anime.ReleasedOn?.DateTime,
                Genres = anime.Genres.Select(i => GetPreferedGenreTitle(config.GenreTitleLanguagePreference, i)).ToArray(),
                CommunityRating = float.Parse(anime.Score),
                OfficialRating = FormatRating(anime.Rating),
                Studios = anime.Studios.Select(i => { return i.Name; }).ToArray(),
                Status = anime.Status switch
                {
                    "released" => SeriesStatus.Ended,
                    "ongoing" => SeriesStatus.Continuing,
                    "anons" => SeriesStatus.Unreleased,
                    _ => null,
                },
                ProviderIds = new Dictionary<string, string> { { ShikimoriPlugin.ProviderName, anime.Id.ToString() } },
            };

            return result;
        }
    }
}
