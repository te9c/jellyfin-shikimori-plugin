using Jellyfin.Plugin.Shikimori.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Shikimori.Api
{
    public class Data
    {
        public Anime[]? animes { get; set; }
    }

    public class GraphQlRequest
    {
        public string? query { get; set; }
    }

    public class GraphQlResponse
    {
        public Data? data { get; set; }
    }

    public class SearchOptions
    {
        public string? search { get; set; }
        public int limit { get; set; } = 1;
        public string? kind { get; set; }
        public string? ids { get; set; }

        // kostil
        public override string ToString()
        {
            List<string> result = new List<string>();
            if (search != null)
            {
                result.Add($"search: \"{search}\"");
            }
            result.Add($"limit: {limit.ToString()}");
            if (kind != null)
            {
                result.Add($"kind: \"{kind}\"");
            }
            if (ids != null)
            {
                result.Add($"ids: \"{ids.ToString()}\"");
            }

            return string.Join(',', result);
        }
    }

    public class Genre
    {
        public long id { get; set; }
        public string? name { get; set; }
        public string? russian { get; set; }
    }

    public class Studio
    {
        public long id { get; set; }
        public string? imageUrl { get; set; }
        public string? name { get; set; }
    }

    public class Poster
    {
        public long id { get; set; }
        public string? originalUrl { get; set; }
        public string? mainUrl { get; set; }
    }

    public class IncompleteDate
    {
        public int? year { get; set; }
        public int? month { get; set; }
        public int? day { get; set; }

        public DateTime? ToDateTime() {
            if (year != null && month != null && day != null)
                return new DateTime(year.Value, month.Value, day.Value);

            return null;
        }
    }

    public class Anime
    {
        public long id { get; set; }

        public string? name { get; set; }
        public string? russian { get; set; }
        public string? japanese { get; set; }
        public string? english { get; set; }

        public string? description { get; set; }
        public string? descriptionHtml { get; set; }
        public string? descriptionSource { get; set; }

        public IncompleteDate? airedOn { get; set; }
        public IncompleteDate? releasedOn { get; set; }

        public Genre[]? genres { get; set; }
        public string? kind { get; set; }

        public float score { get; set; }

        public string? rating { get; set; }

        public Studio[]? studios { get; set; }

        public Poster? poster { get; set; }

        public string? status { get; set; }

        private string? GetPreferedTitle(TitlePreferenceType type)
        {
            // TODO: Fallback languages?
            return type switch
            {
                TitlePreferenceType.Japanese => japanese,
                TitlePreferenceType.Romaji => name,
                TitlePreferenceType.Russian => russian,
                _ => null
            };
        }
        private string? GetPreferedGenreTitle(GenreTitleLanguagePreferenceType type, Genre genre)
        {
            return type switch
            {
                GenreTitleLanguagePreferenceType.English => genre.name,
                GenreTitleLanguagePreferenceType.Russian => genre.russian,
                _ => null
            };
        }

        private string? FormatRating(string? shikimoriRating)
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

        private void FillBaseItem(BaseItem item)
        {
            PluginConfiguration config = ShikimoriPlugin.Instance!.Configuration;

            item.Name = GetPreferedTitle(config.TitlePreference);
            item.OriginalTitle = GetPreferedTitle(config.OriginalTitlePreference);
            item.Overview = descriptionHtml;
            item.ProductionYear = airedOn?.year;
            item.PremiereDate = airedOn?.ToDateTime();
            item.EndDate = releasedOn?.ToDateTime();
            item.Genres = genres?.Select(i => GetPreferedGenreTitle(config.GenreTitleLanguagePreference, i)).ToArray();
            item.CommunityRating = score;
            item.OfficialRating = FormatRating(rating);
            item.Studios = studios?.Select(i => { return i.name; }).ToArray();
            item.ProviderIds = new Dictionary<string, string> { { ShikimoriPlugin.ProviderName, id.ToString() } };
        }

        public RemoteSearchResult ToSearchResult()
        {
            var config = ShikimoriPlugin.Instance!.Configuration;

            var result = new RemoteSearchResult()
            {
                Name = name,
                ProductionYear = airedOn?.year,
                PremiereDate = airedOn?.ToDateTime(),
                ImageUrl = poster?.mainUrl,
                SearchProviderName = ShikimoriPlugin.ProviderName,
                ProviderIds = new Dictionary<string, string>() { { ShikimoriPlugin.ProviderId, id.ToString() } },
            };

            return result;
        }

        public Movie ToMovie()
        {
            var result = new Movie();
            FillBaseItem(result);

            return result;
        }

        public Series ToSeries()
        {
            var result = new Series()
            {
                Status = status switch
                {
                    "released" => SeriesStatus.Ended,
                    "ongoing" => SeriesStatus.Continuing,
                    "anons" => SeriesStatus.Unreleased,
                    _ => null,
                },
            };
            FillBaseItem(result);

            return result;
        }

    }
}