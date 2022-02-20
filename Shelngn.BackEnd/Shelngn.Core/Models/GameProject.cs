namespace Shelngn.Models
{
    public class GameProject
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; } = null!;
        public string FilesLocation { get; set; } = null!;
        public DateTimeOffset InsertDate { get; set; }
    }

    public class GameProjectMember
    {
        public Guid GameProjectId { get; set; }
        public Guid AppUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
    }
}
