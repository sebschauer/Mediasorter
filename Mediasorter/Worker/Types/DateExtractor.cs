using System.Text.RegularExpressions;
using Mediasorter.Model;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Serilog;

namespace Mediasorter.Worker.Types;

public class DateExtractor : BaseUnitOfWork
{
    private readonly string _fromRegex;
    private readonly string _toRegex;

    public DateExtractor(UnitOfWorkModel model, ConfigurationModel configurationModel) : base(model, configurationModel)
    {
        _fromRegex = model.ExtractDate!.From;
        _toRegex = model.ExtractDate.To;
    }

    public override bool DoSpecificWork(FileInfo file)
    {
        var date = GetTakenDateTime(ImageMetadataReader.ReadMetadata(file.FullName));
        if (date == null)
        {
            Log.Verbose("  File {file}: No creation date found in file.", file.Name);
            return true;
        }

        var oldName = file.Name;
        var newToRegex = _toRegex.Replace("{DATE}", $"{date:yyyyMMdd_HHmmss}");
        // newToRegex = Regex.Replace(newToRegex, @"{([^}]+?)}", $"{date.Value:$1}");
        newToRegex = newToRegex.Replace("{d}", $"{date:d}");
        newToRegex = newToRegex.Replace("{dd}", $"{date:dd}");
        newToRegex = newToRegex.Replace("{ddd}", $"{date:ddd}");
        newToRegex = newToRegex.Replace("{dddd}", $"{date:dddd}");
        newToRegex = newToRegex.Replace("{f}", $"{date:f}");
        newToRegex = newToRegex.Replace("{ff}", $"{date:ff}");
        newToRegex = newToRegex.Replace("{fff}", $"{date:fff}");
        newToRegex = newToRegex.Replace("{ffff}", $"{date:ffff}");
        newToRegex = newToRegex.Replace("{fffff}", $"{date:fffff}");
        newToRegex = newToRegex.Replace("{ffffff}", $"{date:ffffff}");
        newToRegex = newToRegex.Replace("{fffffff}", $"{date:fffffff}");
        newToRegex = newToRegex.Replace("{F}", $"{date:F}");
        newToRegex = newToRegex.Replace("{FF}", $"{date:FF}");
        newToRegex = newToRegex.Replace("{FFF}", $"{date:FFF}");
        newToRegex = newToRegex.Replace("{FFFF}", $"{date:FFFF}");
        newToRegex = newToRegex.Replace("{FFFFF}", $"{date:FFFFF}");
        newToRegex = newToRegex.Replace("{FFFFFF}", $"{date:FFFFFF}");
        newToRegex = newToRegex.Replace("{FFFFFFF}", $"{date:FFFFFFF}");
        newToRegex = newToRegex.Replace("{g}", $"{date:g}");
        newToRegex = newToRegex.Replace("{gg}", $"{date:gg}");
        newToRegex = newToRegex.Replace("{h}", $"{date:h}");
        newToRegex = newToRegex.Replace("{hh}", $"{date:hh}");
        newToRegex = newToRegex.Replace("{H}", $"{date:H}");
        newToRegex = newToRegex.Replace("{HH}", $"{date:HH}");
        newToRegex = newToRegex.Replace("{m}", $"{date:m}");
        newToRegex = newToRegex.Replace("{mm}", $"{date:mm}");
        newToRegex = newToRegex.Replace("{M}", $"{date:M}");
        newToRegex = newToRegex.Replace("{MM}", $"{date:MM}");
        newToRegex = newToRegex.Replace("{MMM}", $"{date:MMM}");
        newToRegex = newToRegex.Replace("{MMMM}", $"{date:MMMM}");
        newToRegex = newToRegex.Replace("{s}", $"{date:s}");
        newToRegex = newToRegex.Replace("{ss}", $"{date:ss}");
        newToRegex = newToRegex.Replace("{t}", $"{date:t}");
        newToRegex = newToRegex.Replace("{tt}", $"{date:tt}");
        newToRegex = newToRegex.Replace("{y}", $"{date:y}");
        newToRegex = newToRegex.Replace("{yy}", $"{date:yy}");
        newToRegex = newToRegex.Replace("{yyy}", $"{date:yyy}");
        newToRegex = newToRegex.Replace("{yyyy}", $"{date:yyyy}");
        newToRegex = newToRegex.Replace("{yyyyy}", $"{date:yyyyy}");
        newToRegex = newToRegex.Replace("{z}", $"{date:z}");
        newToRegex = newToRegex.Replace("{zz}", $"{date:zz}");
        newToRegex = newToRegex.Replace("{zzz}", $"{date:zzz}");
        var newName = Regex.Replace(oldName, _fromRegex, newToRegex);

        if (oldName == newName)
            return true;

        try
        {
            file.MoveTo(Path.Combine(file.DirectoryName, newName));
            Log.Verbose("  Renamed '{old}' to '{new}'.", oldName, newName);
            return true;
        }
        catch (Exception)
        {
            Log.Warning("  Could not rename file '{old}' to '{new}'!", oldName, newName);
            return false;
        }
    }

    static DateTime? GetTakenDateTime(IEnumerable<MetadataExtractor.Directory> directories)
    {
        // obtain the Exif SubIFD directory
        var directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

        if (directory is null)
            return null;

        // query the tag's value
        if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime))
            return dateTime;

        return null;
    }
}