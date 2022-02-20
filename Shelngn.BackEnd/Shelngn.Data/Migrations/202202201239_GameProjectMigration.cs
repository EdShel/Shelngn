using FluentMigrator;
using System.Data;

namespace Shelngn.Data.Migrations
{
    [Migration(202202201239)]
    public class GameProjectMigration : Migration
    {
        public override void Up()
        {
            this.Create.Table("game_project")
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("project_name").AsString(256).NotNullable()
                .WithColumn("files_location").AsString().NotNullable().Unique()
                .WithColumn("insert_date").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentDateTimeOffset);

            this.Create.Table("game_project_member")
                .WithColumn("game_project_id").AsGuid().NotNullable().ForeignKey("game_project", "id").OnDelete(Rule.Cascade)
                .WithColumn("app_user_id").AsGuid().NotNullable().ForeignKey("app_user", "id")
                .WithColumn("insert_data").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentDateTimeOffset);
        }

        public override void Down()
        {
            this.Delete.Table("game_project_member");
            this.Delete.Table("game_project");
        }
    }
}
