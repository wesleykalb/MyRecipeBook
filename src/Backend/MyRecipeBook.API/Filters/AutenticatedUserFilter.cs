using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.API.Filters;
public class AutenticatedUserFilter : IAsyncAuthorizationFilter
{
    private readonly IAccessTokenValidator _accessTokenValidator;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    public AutenticatedUserFilter(IAccessTokenValidator accessTokenValidator, IUserReadOnlyRepository userReadOnlyRepository)
    {
        _accessTokenValidator = accessTokenValidator;
        _userReadOnlyRepository = userReadOnlyRepository;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            var token = TokenOnRequest(context);

            var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);

            var userExists = await _userReadOnlyRepository.ExistsActiveUserWithIdentifier(userIdentifier);

            if (!userExists)
            {
                throw new MyRecipeBookException(ResourceMessagesException.USER_WITHOUT_PERMISSION_RESOURCE);
            }
        }
        catch (SecurityTokenExpiredException)
        {
            context.Result =  new UnauthorizedObjectResult(new ResponseErrorJson("Token expired")
            {
                TokenIsExpired = true
            });
        }
        catch (MyRecipeBookException ex)
        {
            context.Result =  new UnauthorizedObjectResult(new ResponseErrorJson(ex.Message));
        }
        catch 
        {
            context.Result =  new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.USER_WITHOUT_PERMISSION_RESOURCE));
        }
        

    }

    private static string TokenOnRequest(AuthorizationFilterContext context)
    {
        var token = context.HttpContext.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new MyRecipeBookException(ResourceMessagesException.NO_TOKEN);
        }
        return token["Bearer ".Length..].Trim();
    }
}