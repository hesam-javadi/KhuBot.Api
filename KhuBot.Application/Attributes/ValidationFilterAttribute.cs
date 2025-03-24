using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Domain.DTOs;
using KhuBot.Domain.Exceptions;
using KhuBot.Domain.Utilities;

namespace KhuBot.Application.Attributes
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Keys
                    .SelectMany(key => context.ModelState[key].Errors.Select(error => new { Key = key, Error = error.ErrorMessage }))
                    .ToList();
                var errorResponseDetails = errors.Select(e => new ErrorResponseDetailDto()
                {
                    ErrorId = null,
                    ErrorMessage = e.Error,
                    ErrorKey = e.Key.TrimStart('$').TrimStart('.').FirstCharToLower(),
                    IsInternalError = false
                }).ToList();
                throw new StatusCodeException(errorResponseDetails, HttpStatusCode.BadRequest);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
