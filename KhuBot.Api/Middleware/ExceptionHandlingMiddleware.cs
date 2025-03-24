using System.Net;
using System.Text.Json;
using KhuBot.Domain.DTOs;
using KhuBot.Domain.Exceptions;
using KhuBot.Domain.Utilities;
using Microsoft.Extensions.Localization;

namespace KhuBot.Api.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (StatusCodeException ex)
            {
                ex.ErrorResponse.ErrorMessages.ForEach(em => em.ErrorKey = em.ErrorKey.FirstCharToLower());
                context.Response.StatusCode = (int)ex.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(ex.ErrorResponse));
            }
            catch (Exception ex)
            {
                var errorCode = Guid.NewGuid().ToString().Replace("-", "");
                logger.LogError(ex, errorCode);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponseDto(
                [
                    new()
                    {
                        ErrorMessage = "خطای فنی رخ داده است! لطفا پس از چند دقیقه دوباره تلاش نمایید.",
                        ErrorId = errorCode,
                        IsInternalError = true
                    }
                ])));
            }
        }
    }
}