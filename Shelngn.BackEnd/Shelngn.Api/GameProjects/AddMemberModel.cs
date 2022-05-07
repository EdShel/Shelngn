using System.ComponentModel.DataAnnotations;

namespace Shelngn.Api.GameProjects
{
    public class AddMemberModel
    {
        [Required]
        public string? EmailOrUserName { get; set; }
    }

}
