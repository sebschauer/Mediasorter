namespace Mediasorter.Tests.Helpers
{
    public class DirectoryHandler
    {
        public static string CreateTestDirectory(string? testId = null, IEnumerable<string>? filenames = null, IEnumerable<FileInfo>? filesToCopy = null)
        {
            var id = testId ?? Guid.NewGuid().ToString();
            var path = Path.Combine(Directory.GetCurrentDirectory(), id);

            if (Directory.Exists(path))
                Directory.Delete(path, true);
            
            Directory.CreateDirectory(path);

            foreach (var filename in filenames ?? new List<string>())
            {
                File.WriteAllText(Path.Combine(path, filename), "");
            }

            foreach (var file in filesToCopy ?? new List<FileInfo>())
            {
                file.CopyTo(Path.Combine(path, file.Name));
            }
            return path;
        }

        public static IEnumerable<string> GetFilenames(string path) =>
            Directory.EnumerateFiles(path).Select(Path.GetFileName)!;
    }
}
