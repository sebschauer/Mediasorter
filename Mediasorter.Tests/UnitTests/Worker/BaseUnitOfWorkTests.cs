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