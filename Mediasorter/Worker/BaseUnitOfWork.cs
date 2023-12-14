﻿using System.Text.RegularExpressions;
using Mediasorter.Model;
using Serilog;

namespace Mediasorter.Worker
{
    public abstract class BaseUnitOfWork : IUnitOfWork
    {
        protected UnitOfWorkModel UnitOfWorkModel;
        protected ConfigurationModel ConfigurationModel;

        protected BaseUnitOfWork(UnitOfWorkModel model, ConfigurationModel configurationModel)
        {
            UnitOfWorkModel = model;
            ConfigurationModel = configurationModel;
        }

        public bool DoWork(string directory)
        {
            var log = $"{UnitOfWorkModel.Index}: {UnitOfWorkModel.Name} ({UnitOfWorkModel.Description})";
            if (log == $"{UnitOfWorkModel.Index} ()")
                log = UnitOfWorkModel.Index.ToString();
            Log.Information("Start working unit {log}", log);

            var hadError = false;
            var touchedFiles = 0;
            var include = UnitOfWorkModel.Include ?? ConfigurationModel.FilterPresets[UnitOfWorkModel.IncludePreset!];
            string exclude = null!;
            if (UnitOfWorkModel.ExcludePreset != null)
                exclude = ConfigurationModel.FilterPresets[UnitOfWorkModel.ExcludePreset];
            if (UnitOfWorkModel.Exclude != null)
                exclude = UnitOfWorkModel.Exclude;

            var files = Directory.EnumerateFiles(directory)
                .Select(f => new FileInfo(f))
                .Where(fi => Regex.IsMatch(fi.Name, include))
                .Where(fi => string.IsNullOrEmpty(exclude) || !Regex.IsMatch(fi.Name, exclude));

            foreach (var file in files)
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

            return !hadError;
        }

        protected abstract bool DoSpecificWork(FileInfo file);
    }
}
