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

using FluentAssertions;
using Mediasorter.Tests.Helpers;

namespace Mediasorter.Tests.EndToEndTests
{
    public class EndToEndTests
    {
        [Fact]
        public void AllMustWork()
        {
            var files = new[]
            {
                "31_12_2023_myOldFile.TXT",
                "01_01_2024_myNewFile.txt",
                "important_01_01_2024_myImportantFile.txt",
                "OldFileToKeep.txt"
            };
            var assertedFilesToRemain = new[]
            {
                "2024_01_01_important___myImportantFile.txt",
                "OldFileToKeep.txt"
            };
            var assertedOldFiles = new[]
            {
                "2023_12_31___myOldFile.txt"
            };
            var assertedNewFiles = new[]
            {
                "2024_01_01___myNewFile.txt"
            };
            var assertedAllFiles = new[]
            {
                "2023_12_31___myOldFile.txt",
                "2024_01_01___myNewFile.txt"
            };

            var testdir = DirectoryHandler.CreateTestDirectory(testId: "E2ETestDirectory", filenames: files);
            var configfile = Path.Combine(Directory.GetCurrentDirectory(), "EndToEndTests", "e2e.config.json");

            Program.Main(new []{ "-path", testdir, "-configfile", configfile });

            DirectoryHandler.GetFilenames(testdir)
                .Should().BeEquivalentTo(assertedFilesToRemain);
            DirectoryHandler.GetFilenames(Path.Combine(testdir, "oldFiles"))
                .Should().BeEquivalentTo(assertedOldFiles);
            DirectoryHandler.GetFilenames(Path.Combine(testdir, "newFiles"))
                .Should().BeEquivalentTo(assertedNewFiles);
            DirectoryHandler.GetFilenames(Path.Combine(testdir, "allFiles"))
                .Should().BeEquivalentTo(assertedAllFiles);

            Directory.Delete(testdir, true);
        }
    }
}
