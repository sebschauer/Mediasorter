namespace Mediasorter.Worker
{
    public interface IUnitOfWork
    {
        bool DoWork(string path);
    }
}
