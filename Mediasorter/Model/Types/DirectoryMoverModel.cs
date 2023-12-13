namespace Mediasorter.Model.Types
{
    public class DirectoryMoverModel
    {
        public List<string>? DirectoriesToMove { get; set; }
        public bool? DeleteAfterMove { get; set; } = false;
    }
}
