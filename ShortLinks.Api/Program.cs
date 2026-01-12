
using Microsoft.EntityFrameworkCore;
using ShortLinks.Core;
using ShortLinks.Storage.EFCore;

namespace ShortLinks.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ✅ 1) Register ShortLinks Core services (service + code generator + options)
            builder.Services.AddShortLinksCore(opt =>
            {
                opt.BaseUrl = "https://localhost:7111/s"; // what will be returned in Create result
                opt.DefaultCodeLength = 8;
                opt.MaxCreateAttempts = 15;
                opt.TrackHits = true;
            });

            // ✅ 2) Register EF Core SQL Server store
            builder.Services.AddShortLinksEfCore(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ShortLinksDb")));

            var app = builder.Build();

            // ✅ 3) Apply migrations automatically (recommended)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ShortLinksDbContext>();
                db.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
