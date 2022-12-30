using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ViaChatServer.BuildingBlocks.Infrastructure.Constants;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;
using ViaChatServer.BuildingBlocks.Infrastructure.Resources;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Attributes
{
    /// <summary>
    /// Filter attribute that will return a bad request response in case the model state isn't valid.
    /// </summary>
    /// <seealso cref="ActionFilterAttribute" />
    public sealed class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new ValidationErrorResponse(ApiDefinitions.GenericValidationKey, ModelValidation.GenericModelValidation, context.ModelState));
            }
        }
    }
}
