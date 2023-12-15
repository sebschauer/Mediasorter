using FluentAssertions;
using Mediasorter.Model;
using Mediasorter.Tests.Helpers;
using Mediasorter.Worker.Types;

namespace Mediasorter.Tests.UnitTests.Worker.Types
{
    public class RegexReplacerTests
    {
        private readonly UnitOfWorkModel _unitModel = new()
        {
            Include = ".*",
            ReplaceRegex = new()
            {
                From = "(.*)_(.*)\\.(.*)",
                To = "$2_$1.$3",
                
            }
        };

        [Fact]
        public void WhenSomethingToReplace_Renames()
        {
            var testDir = DirectoryHandler.CreateTestDirectory(filenames: new[]
            {
                "FIRST_SECOND.extension",
                "anothername.txt"
            });
            var sut = new RegexReplacer(_unitModel, null);

            sut.DoWork(testDir);

            DirectoryHandler.GetFilenames(testDir)
                .Should().BeEquivalentTo(new[]
                {
                    "SECOND_FIRST.extension",
                    "anothername.txt"
                });
            Directory.Delete(testDir, true);
        }
    }
}
