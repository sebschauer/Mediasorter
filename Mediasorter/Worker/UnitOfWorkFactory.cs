using Mediasorter.Model;
using Mediasorter.Worker.Types;

namespace Mediasorter.Worker
{
    public static class UnitOfWorkFactory
    {
        public static BaseUnitOfWork Create(UnitOfWorkModel model, ConfigurationModel config)
        {
            if (model.Replace != null)
                return new Replacer(model, config);

            if (model.ReplaceRegex != null)
                return new RegexReplacer(model, config);

            if (model.Move != null)
                return new DirectoryMover(model, config);

            if (model.ExtractDate != null)
                return new DateExtractor(model, config);
            // TODO ADD NEW TYPE HERE AS WELL

            return null!;
        }
    }
}
