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
        [InlineData("{t}", "")]
        [InlineData("{tt}", "")]
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
