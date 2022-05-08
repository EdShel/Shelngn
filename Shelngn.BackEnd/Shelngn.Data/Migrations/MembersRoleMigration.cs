using FluentMigrator;
using Shelngn.Entities;

namespace Shelngn.Data.Migrations
{
    [Migration(202205081553)]
    public class MembersRoleMigration : Migration
    {
        public override void Up()
        {
            this.Create.PrimaryKey("PK_game_project_id_app_user_id").OnTable("game_project_member").Columns("game_project_id", "app_user_id");
            this.Create.Column("member_role").OnTable("game_project_member").AsString().NotNullable().WithDefaultValue(MemberRole.Owner);
        }

        public override void Down()
        {
            this.Delete.PrimaryKey("PK_game_project_id_app_user_id").FromTable("game_project_member");
            this.Delete.Column("member_role").FromTable("game_project_member");
        }
    }
}
