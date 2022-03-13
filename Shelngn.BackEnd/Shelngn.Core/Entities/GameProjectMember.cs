namespace Shelngn.Entities
{
    public class GameProjectMember
    {
        public Guid GameProjectId { get; set; }
        public Guid AppUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
    }
}
