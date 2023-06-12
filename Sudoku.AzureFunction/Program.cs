using DE.Onnen.Sudoku;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddScoped<IBoard<Cell>, Board>();
        services.AddScoped<ICell, Cell>();
        services.AddScoped<Board>();
        services.AddScoped<Cell>();
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
