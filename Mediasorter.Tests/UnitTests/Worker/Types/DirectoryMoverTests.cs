using Mediasorter.Model;
using Mediasorter.Tests.Helpers;
using Mediasorter.Worker.Types;
using FluentAssertions;

namespace Mediasorter.Tests.UnitTests.Worker.Types
{
    public class DirectoryMoverTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhenSomethingToMove_Moves(bool shallDelete)
        {
            var testDir = DirectoryHandler.CreateTestDirectory(filenames: new[] { "myFile.txt" });
            var subDirs = new List<string> { "Subdir1", "Subdir2" };
            var unitModel = new UnitOfWorkModel()
            {
                Include = ".*",
                Move = new()
                {
                    DirectoriesToMove = subDirs.Select(x => Path.Combine(testDir, x)).ToList(),
                    DeleteAfterMove = shallDelete
                }
            };

            var sut = new DirectoryMover(unitModel, null);

            sut.DoWork(testDir);

            foreach (var dir in unitModel.Move!.DirectoriesToMove)
            {
                DirectoryHandler.GetFilenames(Path.Combine(testDir, dir))
                    .Should().BeEquivalentTo(new[] { "myFile.txt" });
            }
            File.Exists(Path.Combine(testDir, "myFile.txt")).Should().Be(!shallDelete);
            
            Directory.Delete(testDir, true);
        }
    }
}
