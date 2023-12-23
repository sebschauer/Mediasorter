/*
	Copyright 2023 Sebastian Schauer.
	
	This file is part of Mediasorter.
	
	Mediasorter is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
	
	Diese Datei ist Teil von Mediasorter.

    Mediasorter ist Freie Software: Sie können es unter den Bedingungen
    der GNU General Public License, wie von der Free Software Foundation,
    Version 3 der Lizenz oder (nach Ihrer Wahl) jeder neueren
    veröffentlichten Version, weiter verteilen und/oder modifizieren.

    Dieses Programm wird in der Hoffnung bereitgestellt, dass es nützlich sein wird, jedoch
    OHNE JEDE GEWÄHR,; sogar ohne die implizite
    Gewähr der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK.
    Siehe die GNU General Public License für weitere Einzelheiten.

    Sie sollten eine Kopie der GNU General Public License zusammen mit diesem
    Programm erhalten haben. Wenn nicht, siehe <https://www.gnu.org/licenses/>.
*/

using System.Globalization;
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
    private CultureInfo _culture;


    public DateExtractor(UnitOfWorkModel model, ConfigurationModel configurationModel) : base(model, configurationModel)
    {
        _fromRegex = model.ExtractDate!.From;
        _toRegex = model.ExtractDate.To;
        _culture = model.ExtractDate.Culture != null
            ? CultureInfo.CreateSpecificCulture(model.ExtractDate.Culture)
            : CultureInfo.InvariantCulture;
    }

    protected override bool DoSpecificWork(FileInfo file)
    {
        var date = GetTakenDateTime(ImageMetadataReader.ReadMetadata(file.FullName));
        if (date == null)
        {
            Log.Verbose("  File {file}: No creation date found in file.", file.Name);
            return true;
        }

        var oldName = file.Name;
        var newToRegex = _toRegex.Replace("{DATE}", $"{date:yyyyMMdd_HHmmss}");
        newToRegex = newToRegex.Replace("{d}", $"{date:%d}");
        newToRegex = newToRegex.Replace("{dd}", date.Value.ToString("dd", _culture));
        newToRegex = newToRegex.Replace("{ddd}", date.Value.ToString("ddd", _culture));
        newToRegex = newToRegex.Replace("{dddd}", date.Value.ToString("dddd", _culture));
        newToRegex = newToRegex.Replace("{f}", date.Value.ToString("%f", _culture));
        newToRegex = newToRegex.Replace("{ff}", date.Value.ToString("ff", _culture));
        newToRegex = newToRegex.Replace("{fff}", date.Value.ToString("fff", _culture));
        newToRegex = newToRegex.Replace("{ffff}", date.Value.ToString("ffff", _culture));
        newToRegex = newToRegex.Replace("{fffff}", date.Value.ToString("fffff", _culture));
        newToRegex = newToRegex.Replace("{ffffff}", date.Value.ToString("ffffff", _culture));
        newToRegex = newToRegex.Replace("{fffffff}", date.Value.ToString("fffffff", _culture));
        newToRegex = newToRegex.Replace("{F}", date.Value.ToString("%F", _culture));
        newToRegex = newToRegex.Replace("{FF}", date.Value.ToString("FF", _culture));
        newToRegex = newToRegex.Replace("{FFF}", date.Value.ToString("FFF", _culture));
        newToRegex = newToRegex.Replace("{FFFF}", date.Value.ToString("FFFF", _culture));
        newToRegex = newToRegex.Replace("{FFFFF}", date.Value.ToString("FFFFF", _culture));
        newToRegex = newToRegex.Replace("{FFFFFF}", date.Value.ToString("FFFFFF", _culture));
        newToRegex = newToRegex.Replace("{FFFFFFF}", date.Value.ToString("FFFFFFF", _culture));
        newToRegex = newToRegex.Replace("{g}", date.Value.ToString("%g", _culture));
        newToRegex = newToRegex.Replace("{gg}", date.Value.ToString("gg", _culture));
        newToRegex = newToRegex.Replace("{h}", date.Value.ToString("%h", _culture));
        newToRegex = newToRegex.Replace("{hh}", date.Value.ToString("hh", _culture));
        newToRegex = newToRegex.Replace("{H}", date.Value.ToString("%H", _culture));
        newToRegex = newToRegex.Replace("{HH}", date.Value.ToString("HH", _culture));
        newToRegex = newToRegex.Replace("{m}", date.Value.ToString("%m", _culture));
        newToRegex = newToRegex.Replace("{mm}", date.Value.ToString("mm", _culture));
        newToRegex = newToRegex.Replace("{M}", date.Value.ToString("%M", _culture));
        newToRegex = newToRegex.Replace("{MM}", date.Value.ToString("MM", _culture));
        newToRegex = newToRegex.Replace("{MMM}", date.Value.ToString("MMM", _culture));
        newToRegex = newToRegex.Replace("{MMMM}", date.Value.ToString("MMMM", _culture));
        newToRegex = newToRegex.Replace("{s}", date.Value.ToString("%s", _culture));
        newToRegex = newToRegex.Replace("{ss}", date.Value.ToString("ss", _culture));
        newToRegex = newToRegex.Replace("{t}", date.Value.ToString("%t", _culture));
        newToRegex = newToRegex.Replace("{tt}", date.Value.ToString("tt", _culture));
        newToRegex = newToRegex.Replace("{y}", date.Value.ToString("%y", _culture));
        newToRegex = newToRegex.Replace("{yy}", date.Value.ToString("yy", _culture));
        newToRegex = newToRegex.Replace("{yyy}", date.Value.ToString("yyy", _culture));
        newToRegex = newToRegex.Replace("{yyyy}", date.Value.ToString("yyyy", _culture));
        newToRegex = newToRegex.Replace("{yyyyy}", date.Value.ToString("yyyyy", _culture));
        newToRegex = newToRegex.Replace("{z}", date.Value.ToString("%z", _culture));
        newToRegex = newToRegex.Replace("{zz}", date.Value.ToString("zz", _culture));
        newToRegex = newToRegex.Replace("{zzz}", date.Value.ToString("zzz", _culture));
        var newName = Regex.Replace(oldName, _fromRegex, newToRegex);

        if (oldName == newName)
            return true;

        try
        {
            file.MoveTo(Path.Combine(file.DirectoryName!, newName));
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