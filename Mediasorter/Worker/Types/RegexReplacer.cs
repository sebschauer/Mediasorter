using System.Text.RegularExpressions;
using Mediasorter.Model;
using Mediasorter.Model.Types;
using Serilog;

namespace Mediasorter.Worker.Types
{
    public class RegexReplacer : BaseUnitOfWork
    {
        readonly RegexReplacerModel _configuration;

        public RegexReplacer(UnitOfWorkModel model, ConfigurationModel config) : base(model, config)
        {
            _configuration = UnitOfWorkModel.ReplaceRegex!;
        }

        public override bool DoSpecificWork(FileInfo file)
        {
            var oldName = file.Name;
            var newName = Regex.Replace(file.Name, _configuration.From, _configuration.To);

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
