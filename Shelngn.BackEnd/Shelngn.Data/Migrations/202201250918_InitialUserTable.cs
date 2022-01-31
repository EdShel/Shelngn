using FluentMigrator;
using System.Data;

namespace Shelngn.Data.Migrations
{
    [Migration(202201250918)]
    public class InitialUserTable : Migration
    {
        public override void Up()
        {
            this.Create.Table("app_user")
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("email").AsString(256).NotNullable().Unique()
                .WithColumn("user_name").AsString(64).NotNullable().Unique()
                .WithColumn("password_hash").AsString().NotNullable()
                .WithColumn("avatar_url").AsString(256).Nullable()
                .WithColumn("insert_date").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentDateTimeOffset);

            this.Create.Table("refresh_token")
                .WithColumn("value").AsAnsiString().NotNullable().PrimaryKey()
                .WithColumn("user_id").AsGuid().NotNullable().ForeignKey("app_user", "id").OnDelete(Rule.Cascade)
                .WithColumn("expires").AsDateTimeOffset().NotNullable()
                .WithColumn("insert_date").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentDateTimeOffset);
        }

        public override void Down()
        {
            this.Delete.Table("refresh_token");
            this.Delete.Table("app_user");
        }
    }
}
