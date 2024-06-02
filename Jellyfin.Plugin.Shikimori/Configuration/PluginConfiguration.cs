using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Shikimori.Configuration
{
    public enum TitlePreferenceType
    {
        Romaji,
        Russian,
        Japanese
    }

    public enum GenreTitleLanguagePreferenceType
    {
        English,
        Russian
    }
    public class PluginConfiguration : BasePluginConfiguration
    {
        public PluginConfiguration()
        {
            SearchLimit = 10;
            TitlePreference = TitlePreferenceType.Russian;
            OriginalTitlePreference = TitlePreferenceType.Romaji;
            GenreTitleLanguagePreference = GenreTitleLanguagePreferenceType.Russian;
        }
        private int _searchLimit;
        public int SearchLimit
        {
            get => _searchLimit;
            // This limit is because of shikimori api restriction
            set => _searchLimit = (value >= 1 && value <= 50) ? value : _searchLimit;
        }

        public TitlePreferenceType TitlePreference { get; set; }
        public TitlePreferenceType OriginalTitlePreference { get; set; }
        public GenreTitleLanguagePreferenceType GenreTitleLanguagePreference { get; set; }
    }
}
