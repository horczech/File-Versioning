using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PuxTask.Server;

public class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException validationException)
        {
            context.Result = new BadRequestObjectResult(validationException.Message);
            context.ExceptionHandled = true;
        }
        
        if (context.Exception is DirectoryNotFoundException directoryNotFoundException)
        {
            context.Result = new BadRequestObjectResult(directoryNotFoundException.Message);
            context.ExceptionHandled = true;
        }
    }
}