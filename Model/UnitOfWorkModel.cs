using Mediasorter.Model.Types;

namespace Mediasorter.Model;

public class UnitOfWorkModel 
{
    public int Index {get; set; }
    public string? Name {get; set;}
    public string? Description { get; set; }
    public string Filter { get; set; } = null!;

    public ReplacerModel? replace;
}