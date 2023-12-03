using Mediasorter.Model;
using System.Text.RegularExpressions;

namespace mediasorter.Worker
{
    public abstract class BaseUnitOfWork : IUnitOfWork
    {
        protected UnitOfWorkModel _unitOfWorkModel;
        protected ConfigurationModel _configurationModel;

        protected BaseUnitOfWork(UnitOfWorkModel model, ConfigurationModel configurationModel)
        {
            _unitOfWorkModel = model;
            _configurationModel = configurationModel;
        }

        public bool DoWork(string directory)
        {
            var log = $"{_unitOfWorkModel.Name} ({_unitOfWorkModel.Description})";
            if (log == " ()")
                log = _unitOfWorkModel.Index.ToString();
            Serilog.Log.Information("Start working unit {log}", log);

            var filter = _unitOfWorkModel.Filter ?? _configurationModel.FilterPresets[_unitOfWorkModel.FilterPreset];

            var filteredFiles = Directory.EnumerateFiles(directory)
                .Select(f => new FileInfo(f))
                .Where(fi => Regex.IsMatch(fi.Name, filter));
            foreach (var file in filteredFiles)
                if (DoSpecificWork(file) == false)
                    return false;
            return true;
        }

        public abstract bool DoSpecificWork(FileInfo file);
    }
}
