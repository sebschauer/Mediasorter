namespace Mediasorter.Model.Types;

public class DateExtractorModel
{
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
    public string? Culture { get; set; }
}