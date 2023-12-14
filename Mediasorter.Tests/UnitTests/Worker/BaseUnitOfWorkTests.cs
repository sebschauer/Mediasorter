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
            var testId = Guid.NewGuid();
            var testDir = Path.Combine(Directory.GetCurrentDirectory(), testId.ToString());
            Directory.CreateDirectory(testDir);

            var includePattern = "txt$";
            var excludePattern = "^a";
            var unitOfWorkModel = new UnitOfWorkModel { Include = includePattern, Exclude = excludePattern };
            foreach (var file in new[] {"ignore.jpg", "ignore.txt", "afiletoprocess.txt", "afiletoignore.jpg"})
                File.WriteAllText(Path.Combine(testDir, file), "");

            var sut = new DummyWorker(unitOfWorkModel, null!);
            sut.DoWork(testDir);
            Directory.Delete(testDir, true);

            sut.ProcessedFiles
                .Should().HaveCount(1)
                .And.ContainSingle(f => f.Name == "afiletoprocess.txt");
        }
    }
}