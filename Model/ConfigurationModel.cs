namespace mediasorter.Model;

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

            if (action.FilterPreset != null && !FilterPresets.ContainsKey(action.FilterPreset))
            {
                throw new Exception($"Filterpreset '{action.FilterPreset}' unknown!");
            }
        }
    }
}