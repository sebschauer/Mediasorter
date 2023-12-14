﻿using FluentAssertions;
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
