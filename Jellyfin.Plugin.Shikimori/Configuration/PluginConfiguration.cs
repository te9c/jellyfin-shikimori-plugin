using MediaBrowser.Model.Plugins;
using ShikimoriSharp.Bases;

namespace Jellyfin.Plugin.Shikimori.Configuration
{
    public enum SearchTitlePreferenceType
    {
        Romaji,
        Russian
    }
    public class PluginConfiguration : BasePluginConfiguration
    {
        public PluginConfiguration()
        {
            SearchLimit = 10;
            SearchTitlePreference = SearchTitlePreferenceType.Russian;
        }
        private int _searchLimit;
        public int SearchLimit
        {
            get => _searchLimit;
            // This limit is because of shikimori api restriction
            set => _searchLimit = (value >= 1 && value <= 50) ? value : _searchLimit;
        }

        public SearchTitlePreferenceType SearchTitlePreference { get; set; }
    }
}
