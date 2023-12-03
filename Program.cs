using System.Text.Json;
using System.Text.Json.Serialization;
using Mediasorter.Model;
using Serilog;

namespace Mediasorter;
public class Program
{

    public static void Main(string[] args)
    {
        var settings = ParseCliParameters(args);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.File(
                path: Path.Combine(Path.GetDirectoryName(settings.ConfigFile) ?? ".", ".mediasorter.log"), 
                rollOnFileSizeLimit: true)
            .CreateLogger();
        Log.Information("Starting Mediasorter.");
        Log.Verbose($"Loading configuration from '{settings.ConfigFile}', using directory '{settings.FileDirectory}'.");

        try {
            var configJson = File.ReadAllText(settings.ConfigFile);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            var configuration = JsonSerializer.Deserialize<ConfigurationModel>(configJson, options);
        }
        catch (Exception ex) {
            Log.Fatal("Error reading configurationfile {file}! Error: {error}. Exit.", settings.ConfigFile, ex.Message);
            Log.Verbose("Stacktrace: {trace}", ex.StackTrace);
            Log.Verbose("Inner Exception: {inner}", ex.InnerException);
            return;
        }
        
        /*
        var actionFactory = new UnitOfWorkFactory();
        var actions = configuration.Actions
            .OrderBy(action => action.Index)
            .Select(action => actionFactory.GetUnitOfWork(action, configuration.ExtensionLists));
        
        foreach(var action in actions)
            foreach(var file in Directory.EnumerateFiles(settings.FileDirectory).Select(Path.GetFileName))
                action.DoWork(file);
        */
        Log.Information("Done.");
        Log.CloseAndFlush();
    }

    private static Settings ParseCliParameters(string[] args) 
    {
        var settings = new Settings();
        for (int i = 0; i < args.Count(); i++) 
        {
            switch (args[i]) 
            {
                case "-path":
                    i++;
                    settings.FileDirectory = args[i];
                    break;
                case "-configfile":
                    i++;
                    settings.ConfigFile = args[i];
                    break;
                default:
                    throw new ArgumentException($"Unknown parameter '{args[i]}'!");
            }
        }

        return settings;
    }    
}

