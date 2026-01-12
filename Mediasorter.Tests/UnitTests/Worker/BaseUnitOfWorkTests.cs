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

using FluentAssertions;
using Mediasorter.Model;
using Mediasorter.Tests.Helpers;

namespace Mediasorter.Tests.UnitTests.Worker
{
    public class BaseUnitOfWorkTests
    {
        [Fact]
        public void DoWork_MustUseIncludeExcludePatterns()
        {
            var includePattern = "txt$";
            var excludePattern = "^a";
            var files = new[]
            {
                "ignore.jpg",
                "process.txt",
                "afiletoignore.txt",
                "afiletoignore.jpg"
            };
            var testDir = DirectoryHandler.CreateTestDirectory(filenames: files);
            var unitOfWorkModel = new UnitOfWorkModel { Include = includePattern, Exclude = excludePattern };
            var sut = new DummyWorker(unitOfWorkModel, null!);

            var result = sut.DoWork(testDir);
            Directory.Delete(testDir, true);

            result
                .Should().BeTrue();
            sut.ProcessedFiles
                .Should().HaveCount(1)
                .And.ContainSingle(f => f.Name == "process.txt");
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 3)]
        public void DoWork_MustExitOnFirstError(bool workSuccess, int assertedProcessedFiles)
        {
            var includePattern = ".*";
            var files = new[]
            {
                "file1.txt", 
                "file2.txt", 
                "file3.txt"
            };
            var testDir = DirectoryHandler.CreateTestDirectory(filenames: files);
            var unitOfWorkModel = new UnitOfWorkModel { Include = includePattern };
            var sut = new DummyWorker(unitOfWorkModel, null!, doSpecificWorkResult: workSuccess);

            var result = sut.DoWork(testDir);
            Directory.Delete(testDir, true);

            result
                .Should().Be(workSuccess);
            sut.ProcessedFiles
                .Should().HaveCount(assertedProcessedFiles);
        }
    }
}