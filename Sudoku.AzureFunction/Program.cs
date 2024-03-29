﻿using DE.Onnen.Sudoku;
using DE.Onnen.Sudoku.SolveTechniques;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Sudoku.NetCore;

var host = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddScoped<IBoard<Cell>, Board>();
        services.AddScoped<ICell, Cell>();
        services.AddScoped<Board>();
        services.AddScoped<Cell>();
        services.AddScoped<ISolveTechnique<Cell>, LastCandidateInHouseTechiques>();
        services.AddScoped<ISolveTechnique<Cell>, NakedPairTrippleQuad<Cell>>();
        services.AddScoped<ISolveTechnique<Cell>, HiddenPairTripleQuad<Cell>>();
        services.AddScoped<ISolveTechnique<Cell>, LockedCandidates<Cell>>();
        services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            var options = new OpenApiConfigurationOptions
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.1",
                    Title = "Sudoku-Solver",
                    Description = "Sudoku-Solver-API",
                },
                Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                OpenApiVersion = OpenApiVersionType.V3,
                IncludeRequestingHostName = true,
            };
            return options;
        });
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
