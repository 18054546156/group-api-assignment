using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            await CreateGroupsAsync(token, "YourName");
            Console.WriteLine("成功创建5个组");

            // 3. 验证创建的组
            await ValidateGroupsAsync(token);
            Console.WriteLine("成功验证组列表");
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

        // 添加时间戳避免重名
        var timestamp = DateTime.Now.ToString("HHmmss");
        
        for (int i = 1; i <= 5; i++)
        {
            var groupName = $"{firstName}-Group{i}-{timestamp}";
            var content = new StringContent(
                JsonSerializer.Serialize(new { name = groupName }),
                Encoding.UTF8,
                "application/json");

            try
            {
                var response = await client.PostAsync(GroupEndpoint, content);
                var responseBody = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Created group: {groupName}");
                }
                else
                {
                    Console.WriteLine($"Failed to create group {groupName}: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating group {groupName}: {ex.Message}");
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
        response.EnsureSuccessStatusCode();

        try 
        {
            // 修改这里：使用GroupListResponse而不是List<GroupDto>
            var groupResponse = await response.Content.ReadFromJsonAsync<GroupListResponse>();
            Console.WriteLine("Retrieved groups:");
            if (groupResponse?.Items != null)
            {
                foreach (var group in groupResponse.Items)
                {
                    Console.WriteLine($"- {group.Name} (ID: {group.Id})");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JSON 解析错误: {ex.Message}");
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
    public DateTime? LastModificationTime { get; set; }
    public string? LastModifierId { get; set; }
}  