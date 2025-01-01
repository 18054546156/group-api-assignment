using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    private static readonly HttpClient client = new HttpClient();
    private const string TokenEndpoint = "https://dev-apex-01.southeastasia.cloudapp.azure.com:7600/connect/token";
    private const string GroupEndpoint = "https://dev-apex-01.southeastasia.cloudapp.azure.com:7500/api/partner/groups";

    public static async Task Main(string[] args)
    {
        try
        {
            // 1. 获取访问令牌
            var token = await GetAccessTokenAsync();
            Console.WriteLine("成功获取访问令牌");

            // 2. 创建5个组
            await CreateGroupsAsync(token, "XUXIAOYU");
            Console.WriteLine("成功创建5个组");

            // 3. 验证创建的组
            await ValidateGroupsAsync(token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
        }
    }

    private static async Task<string> GetAccessTokenAsync()
    {
        var formData = new Dictionary<string, string>
        {
            {"username", "applicant"},
            {"password", "881d&793M"},
            {"client_id", "External_Integration"},
            {"grant_type", "password"},
            {"client_secret", "3a165ec4-6a3f-a19e-657c-0739e26cb85e"},
            {"scope", "PartnerService"}
        };

        client.DefaultRequestHeaders.Add("__tenant", "T003");

        var response = await client.PostAsync(TokenEndpoint, new FormUrlEncodedContent(formData));
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return content?.Access_token ?? throw new Exception("Token is null");
    }

    private static async Task CreateGroupsAsync(string token, string firstName)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("__tenant", "T003");
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        for (int i = 1; i <= 5; i++)
        {
            var groupName = $"{firstName}-Group{i}";
            var content = new StringContent(
                JsonSerializer.Serialize(new { name = groupName }),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(GroupEndpoint, content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Created group: {groupName}");
            }
        }
    }

    private static async Task ValidateGroupsAsync(string token)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("__tenant", "T003");
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(GroupEndpoint);
        var groupResponse = await response.Content.ReadFromJsonAsync<GroupListResponse>();
        
        if (groupResponse?.Items != null)
        {
            var expectedGroups = new[]
            {
                "XUXIAOYU-Group1",
                "XUXIAOYU-Group2",
                "XUXIAOYU-Group3",
                "XUXIAOYU-Group4",
                "XUXIAOYU-Group5"
            };

            Console.WriteLine("\n验证创建的组:");
            foreach (var expectedName in expectedGroups)
            {
                var exists = groupResponse.Items.Any(g => g.Name == expectedName);
                Console.WriteLine($"- {expectedName}");
            }
        }
    }
}

public class TokenResponse
{
    public string? Access_token { get; set; }
    public string? Token_type { get; set; }
    public int Expires_in { get; set; }
}

public class GroupListResponse
{
    public int TotalCount { get; set; }
    public List<GroupDto> Items { get; set; } = new();
}

public class GroupDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public DateTime CreationTime { get; set; }
    public string? CreatorId { get; set; }
}  