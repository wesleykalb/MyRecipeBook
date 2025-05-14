using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MyRecipeBook.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MyRecipeBookBaseController : ControllerBase
    {
        protected static bool IsNotAuthenticated(AuthenticateResult authenticateResult)
        {
            return authenticateResult.Succeeded.Equals(false)
                || authenticateResult.Principal is null
                || !authenticateResult.Principal.Identities.Any(id => id.IsAuthenticated);
        }
    }
}
