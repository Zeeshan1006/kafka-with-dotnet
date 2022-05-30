using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wikimedia.Producer.Service.Implementations;

namespace Wikimedia.Producer.Service.Configuration;

public static class WikimediaProducerServiceConfiguration
{
    public static void AddWikimediaProducerService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISendMessage, SendMessage>();
    }
}
