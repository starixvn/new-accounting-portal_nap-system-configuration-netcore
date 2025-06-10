using System.Text;

namespace NapAdminCore.Service;

using Newtonsoft.Json;

public class OwpService(HttpClient client)
{
    public async Task<string> CreateUser(int userId, string username)
    {
        var data = new
        {
            UserId = userId,
            Username = username
        };

        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        try
        {
            var response = await client.PostAsync("api/owp/chat/create-account", content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            return e.Message;
        }

        return "OK";
    }
}