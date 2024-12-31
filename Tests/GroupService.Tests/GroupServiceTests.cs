using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;

namespace GroupService.Tests
{
    public class GroupServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly GroupService _groupService;

        public GroupServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _groupService = new GroupService(httpClient, _configurationMock.Object);
        }

        [Fact]
        public async Task GetAccessToken_ShouldReturnToken_WhenApiCallSucceeds()
        {
            // Arrange
            var expectedToken = "test_token";
            var response = new TokenResponse 
            { 
                Access_token = expectedToken,
                Token_type = "Bearer",
                Expires_in = 3600
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(response))
                });

            // Act
            var token = await _groupService.GetAccessTokenAsync();

            // Assert
            Assert.Equal(expectedToken, token);
        }

        [Fact]
        public async Task CreateGroup_ShouldReturnTrue_WhenApiCallSucceeds()
        {
            // Arrange
            var groupName = "Test-Group1";
            
            _httpMessageHandlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new TokenResponse()))
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            var result = await _groupService.CreateGroupAsync(groupName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetGroups_ShouldReturnGroups_WhenApiCallSucceeds()
        {
            // Arrange
            var expectedGroups = new List<GroupDto>
            {
                new GroupDto { Name = "Test-Group1" },
                new GroupDto { Name = "Test-Group2" }
            };

            _httpMessageHandlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new TokenResponse()))
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expectedGroups))
                });

            // Act
            var groups = await _groupService.GetGroupsAsync();

            // Assert
            Assert.Equal(expectedGroups.Count, groups.Count());
            Assert.Equal(expectedGroups[0].Name, groups.First().Name);
        }
    }
} 