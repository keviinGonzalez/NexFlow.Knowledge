using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NexFlow.Knowledge.Application.Abstractions.Chunking;
using NexFlow.Knowledge.Application.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);

                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);

            return services;
        }
    }
}
