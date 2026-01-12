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

using System.Text.RegularExpressions;
using Mediasorter.Model;
using Serilog;

namespace Mediasorter.Worker
{
    public abstract class BaseUnitOfWork : IUnitOfWork
    {
        protected UnitOfWorkModel UnitOfWorkModel;
        protected ConfigurationModel ConfigurationModel;

        protected BaseUnitOfWork(UnitOfWorkModel model, ConfigurationModel configurationModel)
        {
            UnitOfWorkModel = model;
            ConfigurationModel = configurationModel;
        }

        public bool DoWork(string directory)
        {
            var log = $"{UnitOfWorkModel.Index}: {UnitOfWorkModel.Name} ({UnitOfWorkModel.Description})";
            if (log == $"{UnitOfWorkModel.Index} ()")
                log = UnitOfWorkModel.Index.ToString();
            Log.Information("Start working unit {log}", log);

            var hadError = false;
            var touchedFiles = 0;
            var include = UnitOfWorkModel.Include ?? ConfigurationModel.FilterPresets[UnitOfWorkModel.IncludePreset!];
            string exclude = null!;
            if (UnitOfWorkModel.ExcludePreset != null)
                exclude = ConfigurationModel.FilterPresets[UnitOfWorkModel.ExcludePreset];
            if (UnitOfWorkModel.Exclude != null)
                exclude = UnitOfWorkModel.Exclude;

            var files = Directory.EnumerateFiles(directory)
                .Select(f => new FileInfo(f))
                .Where(fi => Regex.IsMatch(fi.Name, include))
                .Where(fi => string.IsNullOrEmpty(exclude) || !Regex.IsMatch(fi.Name, exclude));

            foreach (var file in files)
            {
                if (DoSpecificWork(file) == true)
                {
                    touchedFiles++;
                }
                else
                {
                    hadError = true;
                    break;
                }
            }
            Log.Information("Handled {count} files", touchedFiles);

            return !hadError;
        }

        protected abstract bool DoSpecificWork(FileInfo file);
    }
}
