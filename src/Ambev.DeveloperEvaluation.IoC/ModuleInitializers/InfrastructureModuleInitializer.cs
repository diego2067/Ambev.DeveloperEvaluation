﻿using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();
       
        builder.Services.AddScoped<SaleRepository>();

        builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoDB"));
        builder.Services.AddSingleton<MongoContext>();
        builder.Services.AddScoped<SalesMongoRepository>();
        builder.Services.AddTransient(typeof(MongoRepository<>), typeof(MongoRepository<>));
    }
}