﻿using System.Xml.Serialization;
using MediaBrowser.Common.Plugins;
using Jellyfin.Plugin.Shikimori.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.Shikimori;

public class ShikimoriPlugin : BasePlugin<PluginConfiguration>
{
    public ShikimoriPlugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerialize) : base (applicationPaths, xmlSerialize) {}
    public override string Name => "Shikimori";
    public override Guid Id => Guid.Parse("7EDB2A28-5B8A-4FE8-AE11-D941315EB862");
}