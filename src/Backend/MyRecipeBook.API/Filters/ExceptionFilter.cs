﻿using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is MyRecipeBookException)
                HandleProjectException(context);
            else
                ThrowUnknomException(context);
        }

        private static void HandleProjectException(ExceptionContext context) 
        {
            if (context.Exception is ErrorOnValidationException errorOnValidationException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = new BadRequestObjectResult(new ResponseErrorJson(errorOnValidationException!.ErrorMessages));
            }

            else if (context.Exception is InvalidLoginException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(context.Exception.Message));
            }
            else if (context.Exception is NotFoundException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Result = new NotFoundObjectResult(new ResponseErrorJson(context.Exception.Message));
            }
            else if (context.Exception is RefreshTokenExpiredException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Result = new NotFoundObjectResult(new ResponseErrorJson(context.Exception.Message));
            }
            else if (context.Exception is RefreshTokenNotFoundException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new NotFoundObjectResult(new ResponseErrorJson(context.Exception.Message));
            }
        }

        private static void ThrowUnknomException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesException.UNKNOW_ERROR));
        }
    }
}
