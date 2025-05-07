using FluentMigrator;

namespace MyRecipeBook.Infraestructure.Migrations.Versions;

[Migration(DatabaseVersion.IMAGE_FOR_RECIPES)]
public class Version0000003 : VersionBase
{
    public override void Up()
    {
        Alter.Table("Recipes")
            .AddColumn("ImageIdentifier").AsString().Nullable();
    }
}