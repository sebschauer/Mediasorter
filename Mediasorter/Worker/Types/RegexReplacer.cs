﻿/*
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

using System.Text.RegularExpressions;
using Mediasorter.Model;
using Mediasorter.Model.Types;
using Serilog;

namespace Mediasorter.Worker.Types
{
    public class RegexReplacer : BaseUnitOfWork
    {
        readonly RegexReplacerModel _configuration;

        public RegexReplacer(UnitOfWorkModel model, ConfigurationModel config) : base(model, config)
        {
            _configuration = UnitOfWorkModel.ReplaceRegex!;
        }

        protected override bool DoSpecificWork(FileInfo file)
        {
            var oldName = file.Name;
            var newName = Regex.Replace(file.Name, _configuration.From, _configuration.To);

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
    }
}
