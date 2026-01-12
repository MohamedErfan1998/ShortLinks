using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShortLinks.Abstractions.Interfaces.Repositories;
using ShortLinks.Storage.EFCore.Repositories;

namespace ShortLinks.Storage.EFCore
{
    public static class EfCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddShortLinksEfCore(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<ShortLinksDbContext>(options);
            services.AddScoped<IShortLinkRepository, EfCoreShortLinkRepository>();
            return services;
        }
    }
}