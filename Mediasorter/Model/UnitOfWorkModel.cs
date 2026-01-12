/*
	Copyright 2023-2026 Sebastian Schauer <mediasorter(at)sebschauer.de>.
	
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

using Mediasorter.Model.Types;

namespace Mediasorter.Model;

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
        {
            throw new Exception("Use exactly one of include and includePreset");
        }

        if (Exclude is not null && ExcludePreset is not null)
            throw new Exception("Use only one of exclude and excludePreset");
    }
}