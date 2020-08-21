using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Coramba.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Coramba.Api
{
    public class HttpResponseExceptionFilter: IActionFilter
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public HttpResponseExceptionFilter(ProblemDetailsFactory problemDetailsFactory)
        {
            _problemDetailsFactory = problemDetailsFactory;
        }

        private ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelState)
        {
            return _problemDetailsFactory.CreateValidationProblemDetails(httpContext, modelState);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ValidationException fluentValidationException)
            {
                fluentValidationException
                    .Errors
                    .ForEach(f => { context.ModelState.AddModelError(f.PropertyName, f.ErrorMessage); });
            }
            else if (context.Exception is System.ComponentModel.DataAnnotations.ValidationException validationException)
                context.ModelState.AddModelError("validation", validationException.ValidationResult.ErrorMessage);

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(CreateValidationProblemDetails(context.HttpContext, context.ModelState));
                context.ExceptionHandled = true;
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
