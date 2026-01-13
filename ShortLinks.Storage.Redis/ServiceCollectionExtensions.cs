using Microsoft.Extensions.DependencyInjection;
using ShortLinks.Abstractions.Interfaces.Repositories;
using ShortLinks.Storage.Redis.Options;
using ShortLinks.Storage.Redis.Repositories;
using StackExchange.Redis;

namespace ShortLinks.Storage.Redis
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddShortLinksRedis(
            this IServiceCollection services,
            string connectionString,
            Action<RedisShortLinksOptions>? configure = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Redis connection string is required.", nameof(connectionString));

            var options = new RedisShortLinksOptions();
            configure?.Invoke(options);

            services.AddSingleton(options);

            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                // You can customize ConfigurationOptions here if needed.
                return ConnectionMultiplexer.Connect(connectionString);
            });

            services.AddSingleton<IShortLinkRepository, RedisShortLinkRepository>();

            return services;
        }

        public static IServiceCollection AddShortLinksRedis(
            this IServiceCollection services,
            IConnectionMultiplexer multiplexer,
            Action<RedisShortLinksOptions>? configure = null)
        {
            var options = new RedisShortLinksOptions();
            configure?.Invoke(options);

            services.AddSingleton(options);
            services.AddSingleton(multiplexer);
            services.AddSingleton<IShortLinkRepository, RedisShortLinkRepository>();

            return services;
        }
    }
}