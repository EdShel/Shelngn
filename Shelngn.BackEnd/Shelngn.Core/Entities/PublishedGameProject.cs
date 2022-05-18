namespace Shelngn.Entities
{
    public class PublishedGameProject
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; } = null!;
        public DateTimeOffset InsertDate { get; set; }
        public IList<PublishedGameProjectScreenshot> Screenshots { get; set; } = null!;
        public IList<PublishedGameProjectUserMember> Members { get; set; } = null!;
    }

    public class PublishedGameProjectScreenshot
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = null!;
    }

    public class PublishedGameProjectUserMember
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
    }
}
