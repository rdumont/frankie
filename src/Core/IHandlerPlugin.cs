using RDumont.Frankie.Core.Handlers;

namespace RDumont.Frankie.Core
{
    public interface IHandlerPlugin : IPluginDescriptor
    {
        IAssetHandler CreateHandler(SiteConfiguration configuration, Io io);
    }
}