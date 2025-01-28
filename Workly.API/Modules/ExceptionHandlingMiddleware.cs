using System.Net;
using System.Text.Json;
using Workly.API.Models;
using Workly.Domain.Exceptions;

namespace Workly.API.Modules
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {// Hata loglama
                logger.LogError(ex, "Bir hata oluştu.");

                // Hata yanıtını oluşturma
                var response = context.Response;
                response.ContentType = "application/json";

                // HTTP Durum Kodunu Belirleme
                // Bilinen istisnalara göre durumu değiştirebilirsiniz
                var statusCode = HttpStatusCode.InternalServerError;

                // Özel istisnalar için durum kodlarını ayarlayın
                if (ex is UserAlreadyExistException)
                {
                    statusCode = HttpStatusCode.Conflict; // 409
                    logger.LogError(ex.Message);
                }

                // response durum kodunu ayarlama
                response.StatusCode = (int)statusCode;

                // Hata mesajlarını listeye çevirme
                var errorMessages = new List<string> { ex.Message };

                // İç istisnaları ekleme (isteğe bağlı)
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorMessages.Add(innerException.Message);
                    innerException = innerException.InnerException;
                }

                // ApiResponse oluşturma
                var apiResponse = ApiResponse<string>.Fail("Bir hata oluştu.", errorMessages);

                // JSON olarak yanıtı yazma
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var jsonResponse = JsonSerializer.Serialize(apiResponse, jsonOptions);
                await response.WriteAsync(jsonResponse);
            }
        }
    }

}
