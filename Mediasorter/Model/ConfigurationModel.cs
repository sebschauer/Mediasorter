namespace Mediasorter.Model;

public class ConfigurationModel 
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public List<UnitOfWorkModel> Actions { get; set; } = null!;
    public Dictionary<string, string> FilterPresets { get; set; } = null!;

    public void Validate()
    {
        if (Actions is null || Actions.Count == 0)
        {
            throw new Exception("No action defined, nothing to do!");
        }

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