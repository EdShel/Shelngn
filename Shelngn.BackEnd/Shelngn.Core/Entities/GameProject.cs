namespace Shelngn.Entities
{
    public class GameProject
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; } = null!;
        public string FilesLocation { get; set; } = null!;
        public DateTimeOffset InsertDate { get; set; }
    }
}
