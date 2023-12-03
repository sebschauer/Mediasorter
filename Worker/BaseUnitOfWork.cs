using Mediasorter.Model;
using System.Text.RegularExpressions;

namespace mediasorter.Worker
{
    public abstract class BaseUnitOfWork : IUnitOfWork
    {
        protected UnitOfWorkModel _unitOfWorkModel;

        protected BaseUnitOfWork(UnitOfWorkModel model)
        {
            _unitOfWorkModel = model;
        }

        public bool DoWork(string directory)
        {
            var filteredFiles = Directory.EnumerateFiles(directory)
                .Select(f => new FileInfo(f))
                .Where(fi => Regex.IsMatch(fi.Name, _unitOfWorkModel.Filter));
            foreach (var file in filteredFiles)
                if (DoSpecificWork(file) == false)
                    return false;
            return true;
        }

        public abstract bool DoSpecificWork(FileInfo file);
    }
}
