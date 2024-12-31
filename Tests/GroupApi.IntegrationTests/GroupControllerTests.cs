using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GroupApi.IntegrationTests
{
    public class GroupControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public GroupControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateGroups_ShouldCreateFiveGroups()
        {
            // Arrange
            var firstName = "TestUser";

            // Act
            var response = await _client.PostAsync($"/api/group/create-groups?firstName={firstName}", null);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Successfully created all groups", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task GetGroups_ShouldReturnGroups()
        {
            // Act
            var response = await _client.GetAsync("/api/group");
            var groups = await response.Content.ReadFromJsonAsync<IEnumerable<GroupDto>>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(groups);
            Assert.NotEmpty(groups);
        }
    }
} 