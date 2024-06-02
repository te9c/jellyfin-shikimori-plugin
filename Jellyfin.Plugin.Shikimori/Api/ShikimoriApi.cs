using System.Text;
using Newtonsoft.Json;

namespace Jellyfin.Plugin.Shikimori.Api
{
    public class ShikimoriApi
    {
        // WARNING: Usage of empty or invalid application name could result in ip ban for shikimori!
        // See: https://shikimori.one/oauth
        public string ApplicationName { get; init; }

        private const string ApiLink = $"{ShikimoriPlugin.ShikimoriBaseUrl}/api/graphql";
        private const string Query = @"{
  animes({0}) {
    id

    name
    russian
    japanese
    english

    description
    descriptionHtml
    descriptionSource

    airedOn {
      year
      month
      day
    }
    releasedOn {
      year
      month
      day
    }

    genres {
      id
      name
      russian
    }
    kind

    score

    rating

    studios {
      id
      imageUrl
      name
    }

    poster {
      id
      originalUrl
      mainUrl
    }

    status
  }
}";

        public ShikimoriApi(string applicationName)
        {
            ApplicationName = applicationName;
        }

        public async Task<IEnumerable<Anime>> GetAnimesAsync(SearchOptions options)
        {
            var request = new GraphQlRequest()
            {
                query = Query.Replace("{0}", options.ToString())
            };
            var httpClient = ShikimoriPlugin.Instance!.HttpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", ApplicationName);

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ApiLink, content);

            response.EnsureSuccessStatusCode();

            var graphqlResponse = JsonConvert.DeserializeObject<GraphQlResponse>(await response.Content.ReadAsStringAsync());

            return graphqlResponse?.data?.animes == null ? Enumerable.Empty<Anime>() : graphqlResponse.data.animes;
        }

        public async Task<Anime?> GetAnimeAsync(long id)
        {
            var options = new SearchOptions()
            {
                ids = id.ToString(),
            };

            return (await GetAnimesAsync(options))?.FirstOrDefault();
        }
    }
}
