namespace mediasorter.Worker
{
    public interface IUnitOfWork
    {
        bool DoWork(string path);
    }
}
