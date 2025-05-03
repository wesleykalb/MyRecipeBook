using System.Data;
using FluentMigrator;

namespace MyRecipeBook.Infraestructure.Migrations.Versions;

[Migration(DatabaseVersion.TABLE_RECIPES, "Create a table to save the recipe's information")]
public class Version0000002 : VersionBase
{
    public override void Up()
    {
        CreateTable("Recipes")
            .WithColumn("Title").AsString().NotNullable()
            .WithColumn("CookingTime").AsInt32().Nullable()
            .WithColumn("Difficulty").AsInt32().Nullable()
            .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("FK_Recipe_User_Id", "Users", "Id");

        CreateTable("Ingredients")
            .WithColumn("Item").AsString().NotNullable()
            .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("FK_Ingredient_Recipe_Id", "Recipes", "Id")
            .OnDeleteOrUpdate(Rule.Cascade);
        
        CreateTable("Instructions")
            .WithColumn("Step").AsInt32().NotNullable()
            .WithColumn("Text").AsString(2000).NotNullable()
            .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("FK_Instruction_Recipe_Id", "Recipes", "Id")
            .OnDeleteOrUpdate(Rule.Cascade);
        
        CreateTable("DishTypes")
            .WithColumn("Type").AsInt32().NotNullable()
            .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("FK_Dishtype_Recipe_Id", "Recipes", "Id")
            .OnDeleteOrUpdate(Rule.Cascade);
    }
}
