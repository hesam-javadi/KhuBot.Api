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
                var errorCode = Random.Shared.Next(100000, 999999).ToString();
                logger.LogError(ex, errorCode);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponseDto(
                [
                    new()
                    {
                        ErrorMessage = $"احتمالا یه مشکل فنی داریم! لطفا پس از چند دقیقه دوباره تلاش کن و اگر اوکی نشد کدشو گزارش کن بهمون. کدش: {errorCode}",
                        ErrorId = errorCode,
                        IsInternalError = true
                    }
                ])));
            }
        }
    }
}