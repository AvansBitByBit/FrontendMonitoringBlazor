using FrontendMonitoring.Services;
using FrontendMonitoring.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using HttpRequester;
using HttpRequesterOnlyUrl;

namespace frontendMonitoringUnittesten;

[TestClass]
public class ApiClientTests
{
    [TestMethod]
    public async Task AfvalApiClient_GetDataAsync_ReturnsData()
    {
        // Arrange
        
        // Arrange
        var id = Guid.NewGuid();
        var soort = "plastic";
        var datum = DateTime.Now;
        var tijd = DateTime.Now;
        string location = "avans";
        
        var mockHandler = new Mock<HttpMessageHandler>();        var afvalModel = new AfvalModel
        { 
            Id = id, 
            TrashType = soort, 
            Time = tijd,
            Location = location,
            Latitude = 51.5877167,
            Longitude = 4.7762418,
            Confidence = 0.88
        };
        var json = JsonSerializer.Serialize(afvalModel);
        
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(mockHandler.Object);
        var httpRequester = new HttpRequester.HttpRequester(httpClient);
        var apiClient = new AfvalApiClient(httpRequester);

        // Act
        var result = await apiClient.GetDataAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("plastic", result.TrashType);

    }

    [TestMethod]
    public async Task AfvalApiClient_GetDataAsync_HandlesHttpError()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        };

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(mockHandler.Object);
        var httpRequester = new HttpRequester.HttpRequester(httpClient);
        var apiClient = new AfvalApiClient(httpRequester);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HttpRequestException>(
            async () => await apiClient.GetDataAsync());
    }

    [TestMethod]
    public async Task WeatherApiClient_GetDataAsync_ReturnsData()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var weatherModel = new WeatherModel 
        { 
            Latitude = 52.0,
            Longitude = 5.0,
            Timezone = "Europe/Amsterdam",
            TimezoneAbbreviation = "CET",
            Elevation = 10.0
        };
        var json = JsonSerializer.Serialize(weatherModel);
        
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(mockHandler.Object);
        var httpRequesterOnlyUrl = new HttpRequesterOnlyUrl.HttpRequesterOnlyUrl(httpClient);
        var apiClient = new WeatherApiClient(httpRequesterOnlyUrl);

        // Act
        var result = await apiClient.GetDataAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(52.0, result.Latitude);
        Assert.AreEqual(5.0, result.Longitude);
        Assert.AreEqual("Europe/Amsterdam", result.Timezone);
    }

    [TestMethod]
    public async Task WeatherApiClient_GetDataAsync_HandlesTimeout()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Request timeout"));

        var httpClient = new HttpClient(mockHandler.Object);
        var httpRequesterOnlyUrl = new HttpRequesterOnlyUrl.HttpRequesterOnlyUrl(httpClient);
        var apiClient = new WeatherApiClient(httpRequesterOnlyUrl);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(
            async () => await apiClient.GetDataAsync());
    }

    [TestMethod]
    public async Task WeatherApiClient_GetDataAsync_HandlesNetworkError()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(mockHandler.Object);
        var httpRequesterOnlyUrl = new HttpRequesterOnlyUrl.HttpRequesterOnlyUrl(httpClient);
        var apiClient = new WeatherApiClient(httpRequesterOnlyUrl);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HttpRequestException>(
            async () => await apiClient.GetDataAsync());
    }

    [TestMethod]
    public async Task HttpRequester_GetAsync_MakesCorrectRequest()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var testData = new { Name = "Test", Value = 123 };
        var json = JsonSerializer.Serialize(testData);
        
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri!.ToString().Contains("test-endpoint")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(mockHandler.Object);
        var httpRequester = new HttpRequester.HttpRequester(httpClient);

        // Act
        var result = await httpRequester.GetAsync<dynamic>("test-endpoint");

        // Assert
        Assert.IsNotNull(result);
        mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>());
    }
}