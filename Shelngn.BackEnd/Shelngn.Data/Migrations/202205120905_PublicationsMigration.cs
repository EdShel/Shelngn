using FluentMigrator;

namespace Shelngn.Data.Migrations
{
    [Migration(202205120905)]
    public class PublicationsMigration : Migration
    {
        public override void Up()
        {
            this.Create.Table("game_project_publication")
                .WithColumn("game_project_id").AsGuid().PrimaryKey().NotNullable().ForeignKey("game_project", "id").OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("insert_date").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentDateTimeOffset);
            this.Create.Table("game_project_screenshot")
                .WithColumn("id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("game_project_id").AsGuid().NotNullable().ForeignKey("game_project", "id").OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("image_url").AsString().NotNullable()
                .WithColumn("insert_date").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentDateTimeOffset);
        }

        public override void Down()
        {
            this.Delete.Table("game_project_publication");
            this.Delete.Table("game_project_screenshot");
        }
    }
}
