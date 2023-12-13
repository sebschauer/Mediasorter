using mediasorter.Model.Types;
using Mediasorter.Model.Types;

namespace mediasorter.Model;

public class UnitOfWorkModel 
{
    public int Index {get; set; }
    public string? Name {get; set;}
    public string? Description { get; set; }
    public string? Include { get; set; } = null!;
    public string? IncludePreset { get; set; } = null!;
    public string? Exclude { get; set; } = null!;
    public string? ExcludePreset { get; set; } = null!;
    public ReplacerModel? Replace { get; set; }
    public RegexReplacerModel? ReplaceRegex { get; set; }
    public DateExtractorModel? ExtractDate { get; set; }
    public DirectoryMoverModel? Move { get; set; }

    public void Validate()
    {
        var workers = 0;
        if (Replace != null) 
            workers++;
        if (ReplaceRegex != null) 
            workers++;
        if (Move != null) 
            workers++;
        if (ExtractDate != null)
            workers++;
        // TODO ADD NEW TYPE HERE AS WELL

        if (workers != 1)
            throw new Exception("Each action needs exactly one unit of work (e.g. a replace action)");

        if ((Include is null) == (IncludePreset is null))
            throw new Exception("Use exactly one of filter and filterpreset");

        if (Exclude is not null && ExcludePreset is not null)
            throw new Exception("Use only one of Exclude and ExcludePreset");
    }
}