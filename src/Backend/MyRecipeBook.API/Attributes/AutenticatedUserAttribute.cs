using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Filters;

namespace MyRecipeBook.API.Attributes;
[
    AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)
]
public class AutenticatedUserAttribute : TypeFilterAttribute
{
    public AutenticatedUserAttribute() : base(typeof(AutenticatedUserFilter))
    {
    }
}