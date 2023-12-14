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
        var newTo = _toRegex.Replace("{DATE}", $"{date:yyyyMMdd_HHmmss}");
        var newName = Regex.Replace(oldName, _fromRegex, newTo);

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