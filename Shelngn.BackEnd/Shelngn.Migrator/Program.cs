using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shelngn.Data.Migrations;

CommandType commandType;
switch (args.Length > 0 ? args[0] : "i")
{
    case "i":
        commandType = CommandType.MigrateToLatest;
        break;
    case "u":
        commandType = CommandType.UndoPrevious;
        break;
    case "r":
        commandType = CommandType.RedoPrevious;
        break;
    default:
        throw new Exception("Invalid cmd args.");
}

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .Build();

var connectionString = configuration.GetConnectionString("SqlServer");

var serviceProvider = new ServiceCollection()
    .AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(InitialUserTable).Assembly).For.Migrations()
    )
    .AddLogging(lb => lb.AddFluentMigratorConsole())
    .BuildServiceProvider(validateScopes: false);

using var scope = serviceProvider.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

switch (commandType)
{
    case CommandType.MigrateToLatest:
        runner.MigrateUp();
        break;
    case CommandType.UndoPrevious:
        runner.Rollback(1);
        break;
    case CommandType.RedoPrevious:
        runner.Rollback(1);
        runner.MigrateUp();
        break;
}

enum CommandType
{
    MigrateToLatest,
    UndoPrevious,
    RedoPrevious
};