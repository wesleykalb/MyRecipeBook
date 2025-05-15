using FluentMigrator;

namespace MyRecipeBook.Infraestructure.Migrations.Versions;

[Migration(DatabaseVersion.TABLE_REFRESH_TOKEN)]
public class Version0000004 : VersionBase
{
    public override void Up()
    {
        CreateTable("RefreshTokens")
            .WithColumn("Value").AsString().NotNullable()
            .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("FK_RereshTokens_User_Id", "Users", "Id");
    }
}