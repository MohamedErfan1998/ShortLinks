using Microsoft.Extensions.DependencyInjection;
using ShortLinks.Abstractions.Interfaces.Services;
using ShortLinks.Abstractions.Options;
using ShortLinks.Core.Services;

namespace ShortLinks.Core
{
    public static class ShortLinksCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddShortLinksCore(this IServiceCollection services, Action<ShortLinksOptions>? configure = null)
        {
            if (configure is not null)
                services.Configure(configure);

            services.AddSingleton<ICodeGenerator, Base62CodeGenerator>();
            services.AddScoped<IShortLinkService, ShortLinkService>();

            return services;
        }
    }
}