using Microsoft.OpenApi.Models;

namespace Api.Configurations
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Connect API",
                    Version = "v1",
                    Description = "The API documentation for Connect Social Media Application.",
                    Contact = new OpenApiContact
                    {
                        Name = "Support Team",
                        Email = "maryamtarek.dev@gmail.com",
                        // Url = new Uri("https://connectapi.com")
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            return app;
        }
    }
}