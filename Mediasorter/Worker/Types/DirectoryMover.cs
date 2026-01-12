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

using Mediasorter.Model;
using Serilog;

namespace Mediasorter.Worker.Types
{
    public class DirectoryMover : BaseUnitOfWork
    {
        private readonly List<string> _directories;
        private readonly bool _canDelete;

        public DirectoryMover(UnitOfWorkModel model, ConfigurationModel configurationModel) : base(model, configurationModel)
        {
            _directories = model.Move!.DirectoriesToMove!;
            _canDelete = model.Move.DeleteAfterMove ?? false;
        }

        protected override bool DoSpecificWork(FileInfo file)
        {
            foreach (var directory in _directories)
            {
                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Log.Verbose("  Creating directory {dir}", directory);
                        Directory.CreateDirectory(directory);
                    }
                    file.CopyTo(Path.Combine(directory, file.Name));
                    Log.Verbose("  Copied file '{file}' to '{dir}'.", file.Name, directory);
                }
                catch (Exception ex)
                {
                    Log.Error("Error copying file '{file}' to directory '{dir}', will not delete it!", file.Name, directory);
                    Log.Debug("Error: {err} - {stack}", ex.Message, ex.StackTrace);
                    return true;
                }
            }

            if (_canDelete)
            {
                try
                {
                    file.Delete();
                    Log.Verbose("  Deleted file '{file}'.", file.Name);
                }
                catch (Exception ex)
                {
                    Log.Error("Error removing file '{file}' from source directory '{dir}'!", file.Name, file.DirectoryName);
                    Log.Debug("Error: {err} - {stack}", ex.Message, ex.StackTrace);
                    return true;
                }
            }
            
            return true;
        }
    }
}
