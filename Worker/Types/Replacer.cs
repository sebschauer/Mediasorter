using Mediasorter.Model;
using Mediasorter.Model.Types;
using Serilog;

namespace mediasorter.Worker.Types
{
    public class Replacer : BaseUnitOfWork
    {
        private ReplacerModel _configuration;

        public Replacer(UnitOfWorkModel model) : base(model)
        {
            _configuration = _unitOfWorkModel.Replace!;
        }

        public override bool DoSpecificWork(FileInfo file)
        {
            var oldName = file.Name;
            var newName = file.Name.Replace(_configuration.From, _configuration.To);

            try
            {
                file.MoveTo(Path.Combine(file.DirectoryName, newName));
                Log.Verbose("Renamed '{old}' to '{new}'.", oldName, newName);
                return true;
            }
            catch (Exception)
            {
                Log.Warning("Could not rename file '{old}' to '{new}'!", oldName, newName);
                return false;
            }
        }
    }
}
