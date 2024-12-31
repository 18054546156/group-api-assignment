using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Models;

public class GroupService : IGroupService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    
    public GroupService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var tokenEndpoint = "https://dev-apex-01.southeastasia.cloudapp.azure.com:7600/connect/token";
        
        var formData = new Dictionary<string, string>
        {
            {"username", "applicant"},
            {"password", "881d&793M"},
            {"client_id", "External_Integration"},
            {"grant_type", "password"},
            {"client_secret", "3a165ec4-6a3f-a19e-657c-0739e26cb85e"},
            {"scope", "PartnerService"}
        };

        _httpClient.DefaultRequestHeaders.Add("__tenant", "T003");
        
        var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(formData));
        response.EnsureSuccessStatusCode();
        
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return tokenResponse.Access_token;
    }

    public async Task<bool> CreateGroupAsync(string groupName)
    {
        var token = await GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var endpoint = "https://dev-apex-01.southeastasia.cloudapp.azure.com:7500/api/partner/groups";
        var content = new StringContent(
            JsonSerializer.Serialize(new GroupDto { Name = groupName }), 
            Encoding.UTF8, 
            "application/json");
            
        var response = await _httpClient.PostAsync(endpoint, content);
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<GroupDto>> GetGroupsAsync()
    {
        var token = await GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var endpoint = "https://dev-apex-01.southeastasia.cloudapp.azure.com:7500/api/partner/groups";
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<IEnumerable<GroupDto>>();
    }
} 