using Mediasorter.Model;
using Serilog;

namespace mediasorter.Worker.Types
{
    public class DirectoryMover : BaseUnitOfWork
    {
        private List<string> _directories;
        public DirectoryMover(UnitOfWorkModel model, ConfigurationModel configurationModel) : base(model, configurationModel)
        {
            _directories = model.DirectoriesToMove;
        }

        public override bool DoSpecificWork(FileInfo file)
        {
            foreach (var directory in _directories)
            {
                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Log.Verbose("Creating directory {dir}", directory);
                        Directory.CreateDirectory(directory);
                    }
                    file.CopyTo(Path.Combine(directory, file.Name));
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
