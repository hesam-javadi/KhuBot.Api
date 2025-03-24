using KhuBot.Domain.DTOs;

namespace KhuBot.Domain.Exceptions
{
    public class BaseException(List<ErrorResponseDetailDto> errorResponseDetails)
        : System.Exception(string.Join("\n", errorResponseDetails.Select(e => e.ErrorMessage).ToList()))
    {
        public ErrorResponseDto ErrorResponse { get; set; } = new(errorResponseDetails);
    }
}
