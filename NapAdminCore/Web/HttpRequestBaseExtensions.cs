namespace NapAdminCore.Web;

using Microsoft.AspNetCore.Http;

public static class HttpRequestBaseExtensions
{
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        return request is not null && request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}
