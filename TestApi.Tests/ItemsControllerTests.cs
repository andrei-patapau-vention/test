using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TestApi.Api.Models;

namespace TestApi.Tests;

public class ItemsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ItemsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithItems()
    {
        var response = await _client.GetAsync("/Items");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<Item[]>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsItem()
    {
        var response = await _client.GetAsync("/Items/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var item = await response.Content.ReadFromJsonAsync<Item>();
        Assert.NotNull(item);
        Assert.Equal(1, item.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/Items/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidItem_ReturnsCreated()
    {
        var newItem = new Item { Name = "TestItem", Description = "A test item" };

        var response = await _client.PostAsJsonAsync("/Items", newItem);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<Item>();
        Assert.NotNull(created);
        Assert.Equal("TestItem", created.Name);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task Update_ExistingItem_ReturnsUpdatedItem()
    {
        var updated = new Item { Name = "UpdatedWidget", Description = "Updated description" };

        var response = await _client.PutAsJsonAsync("/Items/1", updated);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<Item>();
        Assert.NotNull(result);
        Assert.Equal("UpdatedWidget", result.Name);
    }

    [Fact]
    public async Task Update_NonExistingItem_ReturnsNotFound()
    {
        var updated = new Item { Name = "Ghost", Description = null };

        var response = await _client.PutAsJsonAsync("/Items/999", updated);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingItem_ReturnsNoContent()
    {
        // First create an item so we can safely delete it without affecting other tests
        var newItem = new Item { Name = "ToDelete", Description = "Will be deleted" };
        var createResponse = await _client.PostAsJsonAsync("/Items", newItem);
        var created = await createResponse.Content.ReadFromJsonAsync<Item>();
        Assert.NotNull(created);

        var response = await _client.DeleteAsync($"/Items/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NonExistingItem_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/Items/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
