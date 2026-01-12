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
using Mediasorter.Model.Types;
using Mediasorter.Tests.Helpers;
using Mediasorter.Worker.Types;

namespace Mediasorter.Tests.UnitTests.Worker.Types
{
    public class DateExtractorTests
    {
        private readonly List<FileInfo> _resources = Directory
            .EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), "UnitTests", "Resources"))
            .Select(f => new FileInfo(f))
            .ToList();

        [Fact]
        public void WhenNoExifData_LeaveFileUntouched()
        {
            var testDir = DirectoryHandler.CreateTestDirectory(filesToCopy: _resources);
            var unitModel = new UnitOfWorkModel
            {
                Include = "_without_",
                ExtractDate = new DateExtractorModel
                {
                    From = "(without)",
                    To = "{DATE}"
                }
            };
            var sut = new DateExtractor(unitModel, null);
            
            sut.DoWork(testDir);

            DirectoryHandler.GetFilenames(testDir)
                .Should().BeEquivalentTo(new[]
                {
                    "image_with_exif.jpg",
                    "image_without_exif.jpg"
                });
            Directory.Delete(testDir, true);
        }

        [Theory]
        [InlineData("{DATE}", "20231231_235957")]
        [InlineData("{d}", "31")]
        [InlineData("{dd}", "31")]
        [InlineData("{ddd}", "So")]
        [InlineData("{dddd}", "Sonntag")]
        [InlineData("{f}", "0")]
        [InlineData("{ff}", "00")]
        [InlineData("{fff}", "000")]
        [InlineData("{ffff}", "0000")]
        [InlineData("{fffff}", "00000")]
        [InlineData("{ffffff}", "000000")]
        [InlineData("{fffffff}", "0000000")]
        [InlineData("{F}", "")]
        [InlineData("{FF}", "")]
        [InlineData("{FFF}", "")]
        [InlineData("{FFFF}", "")]
        [InlineData("{FFFFF}", "")]
        [InlineData("{FFFFFF}", "")]
        [InlineData("{FFFFFFF}", "")]
        [InlineData("{g}", "n. Chr.")]
        [InlineData("{gg}", "n. Chr.")]
        [InlineData("{h}", "11")]
        [InlineData("{hh}", "11")]
        [InlineData("{H}", "23")]
        [InlineData("{HH}", "23")]
        [InlineData("{m}", "59")]
        [InlineData("{mm}", "59")]
        [InlineData("{M}", "12")]
        [InlineData("{MM}", "12")]
        [InlineData("{MMM}", "Dez")]
        [InlineData("{MMMM}", "Dezember")]
        [InlineData("{s}", "57")]
        [InlineData("{ss}", "57")]
        [InlineData("{y}", "23")]
        [InlineData("{yy}", "23")]
        [InlineData("{yyy}", "2023")]
        [InlineData("{yyyy}", "2023")]
        [InlineData("{yyyyy}", "02023")]
        
        public void WhenExifData_RenamesFile(string pattern, string assertedReplacement)
        {
            var testDir = DirectoryHandler.CreateTestDirectory(filesToCopy: _resources);
            var unitModel = new UnitOfWorkModel
            {
                Include = "_with_",
                ExtractDate = new DateExtractorModel
                {
                    From = "(.*)\\.jpg",
                    To = pattern + ".jpg",
                    Culture = "de-DE"
                }
            };
            var sut = new DateExtractor(unitModel, null);

            sut.DoWork(testDir);

            DirectoryHandler.GetFilenames(testDir)
                .Should().Contain($"{assertedReplacement}.jpg")
                .And.Contain("image_without_exif.jpg")
                .And.HaveCount(2);
            Directory.Delete(testDir, true);
        }


        [Theory]
        [InlineData("en-US", "Sunday")]
        [InlineData("de-DE", "Sonntag")]
        [InlineData("fr-FR", "dimanche")]
        [InlineData("es-ES", "domingo")]
        [InlineData(null, "Sunday")]
        public void WhenExifData_UsesCultureInfo(string culture, string assertedReplacement)
        {
            var testDir = DirectoryHandler.CreateTestDirectory(filesToCopy: _resources);
            var unitModel = new UnitOfWorkModel
            {
                Include = "_with_",
                ExtractDate = new DateExtractorModel
                {
                    From = "(.*)\\.jpg",
                    To = "{dddd}.jpg",
                    Culture = culture
                }
            };
            var sut = new DateExtractor(unitModel, null);

            sut.DoWork(testDir);

            DirectoryHandler.GetFilenames(testDir)
                .Should().Contain($"{assertedReplacement}.jpg")
                .And.Contain("image_without_exif.jpg")
                .And.HaveCount(2);
            Directory.Delete(testDir, true);
        }
    }
}
