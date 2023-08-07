namespace NapAdminCore.Web;

using Microsoft.AspNetCore.Http;

public static class HttpRequestBaseExtensions
{
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        return request is { Headers: not null } && request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}
