using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Shikimori.Configuration
{
    public enum TitlePreferenceType
    {
        Romaji,
        Russian
    }
    public class PluginConfiguration : BasePluginConfiguration
    {
        private int _searchLimit = 1;

        public int SearchLimit
        {
            get => _searchLimit;
            // This limit is because of shikimori api restriction
            set => _searchLimit = (value >= 1 && value <= 50) ? value : _searchLimit;
        }
        public TitlePreferenceType TitlePreference { get; set; } = TitlePreferenceType.Russian;
    }
}
