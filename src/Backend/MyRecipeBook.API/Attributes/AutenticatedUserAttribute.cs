using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Filters;

namespace MyRecipeBook.API.Attributes;
public class AutenticatedUserAttribute : TypeFilterAttribute
{
    public AutenticatedUserAttribute() : base(typeof(AutenticatedUserFilter))
    {
    }
}