using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Coramba.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.Api
{
    public class ModelValidationActionFilter : IAsyncActionFilter
    {
        private readonly IValidatorFactory _validatorFactory;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IServiceProvider _serviceProvider;

        public ModelValidationActionFilter(IValidatorFactory validatorFactory, ProblemDetailsFactory problemDetailsFactory, IServiceProvider serviceProvider)
        {
            _validatorFactory = validatorFactory;
            _problemDetailsFactory = problemDetailsFactory;
            _serviceProvider = serviceProvider;
        }

        private ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelState)
        {
            return _problemDetailsFactory.CreateValidationProblemDetails(httpContext, modelState);
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var validatorAttributes =
                    controllerActionDescriptor
                        .ControllerTypeInfo
                        .GetCustomAttributes<ActionValidatorAttribute>()
                        .Concat(controllerActionDescriptor
                            .MethodInfo
                            .GetCustomAttributes<ActionValidatorAttribute>());

                await validatorAttributes
                    .SelectMany(a => a.ValidatorTypes)
                    .Select(t => (IActionValidator)_serviceProvider.GetRequiredService(t))
                    .ForEachAsync(async v =>
                    {
                        await v.ValidateAsync(context);
                    });
            }

            foreach (var (key, value) in context.ActionArguments)
            {
                // skip null values
                if (value == null)
                    continue;

                var validator = _validatorFactory.GetValidator(value.GetType());

                // skip objects with no validators
                if (validator == null)
                    continue;

                // validate
                var result = await validator.ValidateAsync(value);

                result.AddToModelState(context.ModelState, key);
            }

            var isOk = context.ModelState.IsValid;
            if (!isOk)
                context.Result = new BadRequestObjectResult(CreateValidationProblemDetails(context.HttpContext, context.ModelState));
            else
                await next();
        }
    }
}
