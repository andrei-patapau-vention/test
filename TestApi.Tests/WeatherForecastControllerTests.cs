using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TestApi.Api.Models;

namespace TestApi.Tests;

public class WeatherForecastControllerTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetWeatherForecast_ReturnsOk()
    {
        var response = await _client.GetAsync("/WeatherForecast");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsFiveItems()
    {
        var forecasts = await _client.GetFromJsonAsync<WeatherForecast[]>("/WeatherForecast");

        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Length);
    }

    [Fact]
    public async Task GetWeatherForecast_EachItemHasValidTemperatureF()
    {
        var forecasts = await _client.GetFromJsonAsync<WeatherForecast[]>("/WeatherForecast");

        Assert.NotNull(forecasts);
        foreach (var forecast in forecasts)
        {
            int expectedF = 32 + (int)(forecast.TemperatureC / 0.5556);
            Assert.Equal(expectedF, forecast.TemperatureF);
        }
    }
}
