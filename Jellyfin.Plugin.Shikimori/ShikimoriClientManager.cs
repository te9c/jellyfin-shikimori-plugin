using Microsoft.Extensions.Logging;
using ShikimoriSharp;
using ShikimoriSharp.Bases;

namespace Jellyfin.Plugin.Shikimori
{
    public class ShikimoriClientManager
    {
        private ShikimoriClient _shikimoriClient;
        private ILogger _logger;

        public ShikimoriClientManager(ILogger logger)
        {
            // TODO: Think about logger.
            // Maybe it's not a good idea to pass main logger

            // TODO: Maybe move constants like name and client id to other file?

            _shikimoriClient = new ShikimoriClient(logger, new ClientSettings(
                        "Shikimori Jellyfin plugin",
                        "4jL1c_MSgZ4qjC8yNotwYGXQmhJ9wFCukQMm_48vGCY",
                        ""
                        ));

            _logger = logger;
        }
    }
}
