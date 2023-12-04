using Serilog;
using System.Text.RegularExpressions;
using mediasorter.Model;

namespace mediasorter.Worker
{
    public abstract class BaseUnitOfWork : IUnitOfWork
    {
        protected UnitOfWorkModel _unitOfWorkModel;
        protected ConfigurationModel? _configurationModel;

        protected BaseUnitOfWork(UnitOfWorkModel model, ConfigurationModel? configurationModel)
        {
            _unitOfWorkModel = model;
            _configurationModel = configurationModel;
        }

        public bool DoWork(string directory)
        {
            var log = $"{_unitOfWorkModel.Index}: {_unitOfWorkModel.Name} ({_unitOfWorkModel.Description})";
            if (log == $"{_unitOfWorkModel.Index} ()")
                log = _unitOfWorkModel.Index.ToString();
            Serilog.Log.Information("Start working unit {log}", log);

            var hadError = false;
            var touchedFiles = 0;
            var filter = _unitOfWorkModel.Filter ?? _configurationModel.FilterPresets[_unitOfWorkModel.FilterPreset];

            var filteredFiles = Directory.EnumerateFiles(directory)
                .Select(f => new FileInfo(f))
                .Where(fi => Regex.IsMatch(fi.Name, filter));
            foreach (var file in filteredFiles) 
            {
                if (DoSpecificWork(file) == true)
                {
                    touchedFiles++;
                }
                else 
                {
                    hadError = true;
                    break;
                }
            }                
            Log.Information("Handled {count} files", touchedFiles);

            return hadError;
        }

        public abstract bool DoSpecificWork(FileInfo file);
    }
}
