using System.Net;
using KhuBot.Domain.DTOs;

namespace KhuBot.Domain.Exceptions
{
    public class StatusCodeException(List<ErrorResponseDetailDto> errorResponseDetails, HttpStatusCode statusCode)
        : BaseException(errorResponseDetails)
    {
        public HttpStatusCode StatusCode { get; set; } = statusCode;
    }
}
