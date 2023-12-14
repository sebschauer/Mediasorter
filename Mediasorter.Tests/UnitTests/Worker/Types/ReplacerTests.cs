using FluentAssertions;
using Mediasorter.Model;
using Mediasorter.Tests.Helpers;
using Mediasorter.Worker.Types;

namespace Mediasorter.Tests.UnitTests.Worker.Types
{
    public class ReplacerTests
    {
        private readonly UnitOfWorkModel _unitModel = new()
        {
            Include = ".*",
            Replace = new()
            {
                From = "ORIGINAL",
                To = "REPLACED"
            }
        };

        [Fact]
        public void WhenSomethingToReplace_Renames()
        {
            var testDir = DirectoryHandler.CreateTestDirectory(filenames: new[]
            {
                "myORIGINALname.txt", 
                "anothername.txt"
            });
            var sut = new Replacer(_unitModel, null);

            sut.DoWork(testDir);

            DirectoryHandler.GetFilenames(testDir)
                .Should().BeEquivalentTo(new[]
                {
                    "myREPLACEDname.txt",
                    "anothername.txt"
                });
            Directory.Delete(testDir, true);
        }
    }
}
