using mediasorter.Model;
using Serilog;

namespace mediasorter.Worker.Types
{
    public class DirectoryMover : BaseUnitOfWork
    {
        private readonly List<string> _directories;

        public DirectoryMover(UnitOfWorkModel model, ConfigurationModel configurationModel) : base(model, configurationModel)
        {
            _directories = model.DirectoriesToMove!;
        }

        public override bool DoSpecificWork(FileInfo file)
        {
            foreach (var directory in _directories)
            {
                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Log.Verbose("  Creating directory {dir}", directory);
                        Directory.CreateDirectory(directory);
                    }
                    file.CopyTo(Path.Combine(directory, file.Name));
                    Log.Verbose("  Copied file '{file}' to '{dir}'.", file.Name, directory);
                }
                catch (Exception ex)
                {
                    Log.Error("Error copying file '{file}' to directory '{dir}'!", file.Name, directory);
                    return false;
                }
            }
            try
            {
                file.Delete();
                Log.Verbose("  Deleted file '{file}'.", file.Name);
            }
            catch (Exception ex)
            {
                Log.Error("Error removing file '{file}' from source directory '{dir}'!", file.Name, file.DirectoryName);
                return false;
            }
            return true;
        }
    }
}
