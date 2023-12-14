using Mediasorter.Model;
using Mediasorter.Worker;

namespace Mediasorter.Tests.Helpers
{
    public class DummyWorker : BaseUnitOfWork
    {
        public List<FileInfo> ProcessedFiles = new();
        public bool DoSpecificWorkResult;

        public DummyWorker(UnitOfWorkModel model, ConfigurationModel configurationModel, bool doSpecificWorkResult = true) : base(model, configurationModel)
        {
            DoSpecificWorkResult = doSpecificWorkResult;
        }

        protected override bool DoSpecificWork(FileInfo file)
        {
            ProcessedFiles.Add(file);
            return DoSpecificWorkResult;
        }
    }
}
