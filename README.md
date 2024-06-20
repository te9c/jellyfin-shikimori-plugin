[ENG](README_ENG.md)
<h1 align="center">Jellyfin shikimori plugin</h1>
<img align="center" src="https://shikimori.one/assets/layouts/l-top_menu-v2/glyph.svg" />

# О плагине

Этот плагин загружает метаданные с сайта [Shikimori](https://shikimori.one)

Плагин поддерживает загрузку метадаты для аниме фильмов и сериалов. Загружает
рейтинг, описание, постеры, жанры, рейтинг.

# Установка

1. Зайти в Администрирование -> Панель -> Плагины -> Репозитории.
2. Добавить новый репозиторий, в поле "URL репозитория" ввести: `https://raw.githubusercontent.com/te9c/jellyfin-shikimori-plugin/main/manifest.json`.
3. Установить плагин через каталог.

# Сборка

1. Установить [.NET8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2. Склонировать этот репозиторий
3. Скомпилировать плагин следующей командой:
```bash
dotnet publish --configuration Release --output bin
```
4. Создать директорию `shikimori` в папке с плагинами джелифина (Она находится в [директории конфигурации](https://jellyfin.org/docs/general/administration/configuration#configuration-directory))
5. Переместить dll `Jellyfin.Plugin.Shikimori.dll` и `Newtonsoft.Json.dll` из директории `bin` в `shikimori`, созданной в предыдущем шаге.
