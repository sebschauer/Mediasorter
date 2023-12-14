namespace Mediasorter.Model;

public class ConfigurationModel 
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public List<UnitOfWorkModel> Actions { get; set; }
    public Dictionary<string, string> FilterPresets { get; set; }

    public void Validate()
    {
        foreach(var action in Actions)
        {
            action.Validate();

            if (action.IncludePreset != null && !FilterPresets.ContainsKey(action.IncludePreset))
            {
                throw new Exception($"Preset '{action.IncludePreset}' unknown!");
            }
            if (action.ExcludePreset != null && !FilterPresets.ContainsKey(action.ExcludePreset))
            {
                throw new Exception($"Preset '{action.IncludePreset}' unknown!");
            }
        }
    }
}