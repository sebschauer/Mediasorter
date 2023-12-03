using mediasorter.Worker.Types;
using Mediasorter.Model;

namespace mediasorter.Worker
{
    public static class UnitOfWorkFactory
    {
        public static BaseUnitOfWork Create(UnitOfWorkModel model)
        {
            if (model.Replace != null)
            {
                return new Replacer(model);
            }

            return null;
        }
    }
}
