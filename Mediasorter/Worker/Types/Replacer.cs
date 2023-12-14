using Mediasorter.Model;
using Mediasorter.Model.Types;
using Serilog;

namespace Mediasorter.Worker.Types
{
    public class Replacer : BaseUnitOfWork
    {
        private readonly ReplacerModel _configuration;

        public Replacer(UnitOfWorkModel model, ConfigurationModel config) : base(model, config)
        {
            _configuration = UnitOfWorkModel.Replace!;
        }

        protected override bool DoSpecificWork(FileInfo file)
        {
            var oldName = file.Name;
            var newName = file.Name.Replace(_configuration.From, _configuration.To);

            if (oldName == newName)
                return true;

            try
            {
                file.MoveTo(Path.Combine(file.DirectoryName!, newName));
                Log.Verbose("  Renamed '{old}' to '{new}'.", oldName, newName);
                return true;
            }
            catch (Exception)
            {
                Log.Warning("  Could not rename file '{old}' to '{new}'!", oldName, newName);
                return false;
            }
        }
    }
}
