using System.Text.Json;
using System.Text.Json.Serialization;
using Mediasorter.Model;
using Mediasorter.Worker;
using Serilog;

namespace Mediasorter;
public class Program
{

    public static void Main(string[] args)
    {
        Settings settings;
        try
        {
            settings = ParseCliParameters(args);
        }
        catch (Exception ex)
        {
            Usage();
            Console.WriteLine(ex.Message);
            return;
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
            .WriteTo.File(
                path: Path.Combine(Path.GetDirectoryName(settings.ConfigFile) ?? ".", ".mediasorter.log"), 
                rollOnFileSizeLimit: true)
            .CreateLogger();
        Log.Information("Starting Mediasorter.");
        Log.Verbose($"Loading configuration from '{settings.ConfigFile}', using directory '{settings.FileDirectory}'.");

        ConfigurationModel configuration;

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

            configuration = JsonSerializer.Deserialize<ConfigurationModel>(configJson, options);
            configuration!.Validate();
        }
        catch (Exception ex) {
            Log.Fatal("Error reading configurationfile {file}! Error: {error}. Exit.", settings.ConfigFile, ex.Message);
            Log.Verbose("Stacktrace: {trace}", ex.StackTrace);
            Log.Verbose("Inner Exception: {inner}", ex.InnerException);
            return;
        }
        
        var actions = configuration.Actions
            .OrderBy(model => model.Index)
            .Select(model => UnitOfWorkFactory.Create(model, configuration));
        
        foreach(var action in actions)
            action.DoWork(settings.FileDirectory);

        Log.Information("Done.");
        Log.CloseAndFlush();
    }

    private static void Usage() 
    {
        Console.WriteLine($"Usage: {Path.GetFileName(AppDomain.CurrentDomain.FriendlyName)} -path <path> -configfile <file>");
        Console.WriteLine("   -path <path>        The directory where the files to be handled are located");
        Console.WriteLine("   -configfile <file>  Path to the JSON config file where the actions to be done are described.");
    }

    private static Settings ParseCliParameters(string[] args) 
    {
        var settings = new Settings();
        var hasPath = false;
        var hasConfig = false;
        for (int i = 0; i < args.Count(); i++) 
        {
            switch (args[i]) 
            {
                case "-path":
                case "--path":
                    i++;
                    settings.FileDirectory = args[i];
                    hasPath = true;
                    break;
                case "-configfile":
                case "--configfile":
                    i++;
                    settings.ConfigFile = args[i];
                    hasConfig = true;
                    break;
                default:
                    throw new ArgumentException($"Unknown parameter '{args[i]}'!");
            }
        }

        if (!hasConfig || !hasPath)
            throw new ArgumentException("Need both parameters!");

        return settings;
    }    
}

