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

namespace Mediasorter.Model;

public class ConfigurationModel 
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public List<UnitOfWorkModel> Actions { get; set; } = null!;
    public Dictionary<string, string> FilterPresets { get; set; } = null!;

    public void Validate()
    {
        if (Actions is null || Actions.Count == 0)
        {
            throw new Exception("No action defined, nothing to do!");
        }

        foreach(var action in Actions)
        {
            action.Validate();

            if (action.IncludePreset != null && !FilterPresets.ContainsKey(action.IncludePreset))
            {
                throw new Exception($"Preset '{action.IncludePreset}' unknown!");
            }
            if (action.ExcludePreset != null && !FilterPresets.ContainsKey(action.ExcludePreset))
            {
                throw new Exception($"Preset '{action.IncludePreset}' unknown!");
            }
        }
    }
}