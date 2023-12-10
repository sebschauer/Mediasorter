namespace mediasorter.Model.Types;

public class DateExtractorModel
{
    public string From {get; set; }
    public string To {get; set; }

    public string? Exclude { get; set; }
}