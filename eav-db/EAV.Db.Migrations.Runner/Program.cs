using System.Reflection;
using EAV.Db.Migrations.Runner.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace EAV.Db.Migrations.Runner;

internal class Program
{
    internal static void Main(string[] args)
    {
        try
        {
            new Program().Run(args);
            Console.WriteLine("Done.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private static string GetConfigDir()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var dir = new FileInfo(assembly.Location).Directory!;
        while (dir.Parent != null && dir.Name != ".local")
        {
            dir = dir.Parent;
        }

        if (dir.Parent == null)
            throw new ApplicationException("No config dir");

        return dir.FullName;
    }

    private void Run(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder
            .SetBasePath(GetConfigDir())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables();
        var config = builder.Build();

        var connStr = config.GetConnectionString("EAV");
        Console.WriteLine(connStr);

        CreateDatabase(connStr);

        using var serviceProvider = CreateServices(connStr);
        using var scope = serviceProvider.CreateScope();
        RunMigrations(scope.ServiceProvider);
    }

    private ServiceProvider CreateServices(string connStr)
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb =>
                rb.AddPostgres()
                    .WithGlobalConnectionString(connStr)
                    .WithMigrationsIn(typeof(UpMigration).Assembly)
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }

    private void CreateDatabase(string connStr)
    {
        if (DatabaseExists(connStr))
            return;

        var connStrBuilder = new NpgsqlConnectionStringBuilder(connStr);
        var dbName = connStrBuilder.Database;

        connStrBuilder.Database = "postgres";
        connStr = connStrBuilder.ConnectionString;

        using var conn = new NpgsqlConnection(connStr);
        conn.Open();

        using var command = conn.CreateCommand();
        command.CommandText = $"""
            CREATE DATABASE "{dbName}" 
            WITH OWNER = postgres 
            ENCODING = 'UTF8' 
            CONNECTION LIMIT = -1;
            """;

        command.ExecuteNonQuery();
    }

    private bool DatabaseExists(string connStr)
    {
        var connStrBuilder = new NpgsqlConnectionStringBuilder(connStr);
        var dbName = connStrBuilder.Database;

        connStrBuilder.Database = "postgres";
        connStr = connStrBuilder.ConnectionString;

        using var conn = new NpgsqlConnection(connStr);
        conn.Open();

        using var command = conn.CreateCommand();
        command.CommandText =
            $"SELECT DATNAME FROM pg_catalog.pg_database WHERE DATNAME = '{dbName}'";

        var result = command.ExecuteScalar();
        return result != null && result.ToString().Equals(dbName);
    }

    private void RunMigrations(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
