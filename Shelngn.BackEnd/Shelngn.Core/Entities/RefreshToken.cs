namespace Shelngn.Entities
{
    public class RefreshToken
    {
        public string Value { get; set; } = null!;

        public Guid UserId { get; set; }

        public DateTimeOffset Expires { get; set; }

        public DateTimeOffset InsertDate { get; set; }
    }
}
