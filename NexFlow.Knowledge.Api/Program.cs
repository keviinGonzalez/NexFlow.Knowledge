
using NexFlow.Knowledge.Api.Extensions;
using NexFlow.Knowledge.Infrastructure.AI.Options;
using NexFlow.Knowledge.Infrastructure.DependencyInjection;
using NexFlow.Knowledge.Application.DependencyInjection;

namespace NexFlow.Knowledge.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();
            builder.Services.AddSwaggerDocumentation(builder.Configuration);

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Temporary local test bypass: this environment has no HTTPS development certificate.
            // app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
