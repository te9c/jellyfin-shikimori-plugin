<h1 align="center">Jellyfin shikimori plugin</h1>
<img align="center" src="https://shikimori.one/assets/layouts/l-top_menu-v2/glyph.svg" />

# About

This plugin adds the metadata provider for [Shikimori](https://shikimori.one)

# Installation

1. Go to Jellyfin's dashboard -> Plugins -> Repositories.
2. Add new repositoriy with url: `https://raw.githubusercontent.com/te9c/jellyfin-shikimori-plugin/main/manifest.json`.
3. Install Shikimori plugin from the catalogue.

# Build

1. Build plugin with the following command:

```bash
dotnet publish --configuration Release --output bin
```

2. Move dll's `Jellyfin.Plugin.Shikimori.dll` and `Newtonsoft.Json.dll`
   from the `bin` folder to jellyfin's `plugin/shikimori` (you probably need to create that folder)

# Releasing

The best way to release is to use [JPRM](https://github.com/oddstr13/jellyfin-plugin-repository-manager), which can package the plugin
and add it to the `manifest.json`.
