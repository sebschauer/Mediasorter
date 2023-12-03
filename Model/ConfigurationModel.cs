namespace Mediasorter.Model;

public class ConfigurationModel 
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    IList<UnitOfWorkModel> Actions { get; set; } = null!;
    Dictionary<string, string> FilterPresets { get; set; }
}