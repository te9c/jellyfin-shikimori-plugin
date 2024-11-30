using System.Text.RegularExpressions;
using Jellyfin.Extensions;
using Jellyfin.Plugin.Shikimori;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;

public class ProviderIdResolver {
    // Maybe it's better to move file name id search into different class.
    public bool TryResolve(IHasProviderIds info, out long id) {
        id = -1;

        // try to find id in local metadata
        var aid = info.ProviderIds.GetValueOrDefault(ShikimoriPlugin.ProviderId);
        if (!String.IsNullOrEmpty(aid) && long.TryParse(aid, out id)) {
            return true;
        }

        // try to find id in file name
        if (info is ItemLookupInfo itemLookupInfo) {
            const string regexPattern = @"\[shikimori-?(\d+)\]";
            Regex regex = new Regex(regexPattern, RegexOptions.RightToLeft | RegexOptions.IgnoreCase);

            string? idString = regex.Match(itemLookupInfo.Path)?.Groups.Values.ElementAtOrDefault(1)?.Value;
            if (!String.IsNullOrEmpty(idString) && long.TryParse(idString, out id)) {
                return true;
            }
        }

        return false;
    }
}
