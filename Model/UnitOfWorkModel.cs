using mediasorter.Model.Types;

namespace mediasorter.Model;

public class UnitOfWorkModel 
{
    public int Index {get; set; }
    public string? Name {get; set;}
    public string? Description { get; set; }
    public string Filter { get; set; } = null!;
    public string FilterPreset { get; set; } = null!;
    public ReplacerModel? Replace { get; set; }
    public RegexReplacerModel? ReplaceRegex { get; set; }
    public List<string>? DirectoriesToMove { get; set; }

    public void Validate()
    {
        var workers = 0;
        if (Replace != null) 
            workers++;
        if (ReplaceRegex != null) 
            workers++;
        if (DirectoriesToMove != null) 
            workers++;
        // TODO ADD NEW TYPE HERE AS WELL

        if (workers != 1)
            throw new Exception("Each action needs exactly one unit of work (e.g. a replace action)");

        if ((Filter is null) == (FilterPreset is null))
            throw new Exception("Use exactly one of filter and filterpreset");
    }
}