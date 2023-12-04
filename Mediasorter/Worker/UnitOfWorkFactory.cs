using mediasorter.Model;
using mediasorter.Worker.Types;

namespace mediasorter.Worker
{
    public static class UnitOfWorkFactory
    {
        public static BaseUnitOfWork Create(UnitOfWorkModel model, ConfigurationModel config)
        {
            if (model.Replace != null)
                return new Replacer(model, config);

            if (model.ReplaceRegex != null)
                return new RegexReplacer(model, config);

            if (model.DirectoriesToMove != null)
                return new DirectoryMover(model, config);
            // TODO ADD NEW TYPE HERE AS WELL

            return null;
        }
    }
}
