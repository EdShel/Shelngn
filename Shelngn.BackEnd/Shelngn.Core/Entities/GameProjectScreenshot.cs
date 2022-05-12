namespace Shelngn.Entities
{
    public class GameProjectScreenshot
    {
        public Guid Id { get; set; }
        public Guid GameProjectId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTimeOffset InsertDate { get; set; }
    }
}
