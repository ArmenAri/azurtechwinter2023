using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestcontainersATW.DataTransfer;
using TestcontainersATW.Errors;
using TestcontainersATW.Persistence;

namespace TestcontainersATW.IT.Custom;

public class CustomImageTests : IClassFixture<AzurTechWinterApiFactory>
{
    private readonly HttpClient _client;

    public CustomImageTests(AzurTechWinterApiFactory factory)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://127.0.0.1:3000")
        };
    }

    // [Fact]
    // public async Task GetAsync_ShouldReturnSecretMessage_WhenTheContainerIsReady()
    // {
    //     // Arrange
    //     // Act
    //     var response = await _client.GetAsync("hello");
    //
    //     // Assert
    //     response.EnsureSuccessStatusCode();
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    //
    //     var result = await response.Content.ReadAsStringAsync();
    //
    //     result.Should().NotBeNull();
    //     result!.Should().Be("Hello Azur Tech Winter 2023");
    // }
}