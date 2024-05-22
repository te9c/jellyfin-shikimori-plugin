<h1 align="center">Jellyfin shikimori plugin</h1>
<img align="center" src="https://shikimori.one/assets/layouts/l-top_menu-v2/glyph.svg" />

# About

This plugin adds the metadata provider for [Shikimori](https://shikimori.one)

# Build

1. Build plugin with the following command:

```bash
dotnet publish --configuration Release --output bin
```

2. Move dll's `Jellyfin.Plugin.Shikimori.dll`, `Newtonsoft.Json.dll`, `Polly.dll`, `ShikimoriSharp.dll`
   from the `bin` folder to jellyfin's `plugin/shikimori` (you probably need to create that folder)
