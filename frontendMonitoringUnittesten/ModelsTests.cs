using FrontendMonitoring.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace frontendMonitoringUnittesten;

[TestClass]
public class ModelsTests
{
    [TestMethod]
    public void LoginModel_Properties_SetAndGetCorrectly()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        
        // Act
        var loginModel = new LoginModel
        {
            Email = email,
            Password = password
        };
        
        // Assert
        Assert.AreEqual(email, loginModel.Email);
        Assert.AreEqual(password, loginModel.Password);
    }

    [TestMethod]
    public void RegisterModel_Properties_SetAndGetCorrectly()
    {
        // Arrange
        var email = "register@example.com";
        var password = "securePassword123";
        
        // Act
        var registerModel = new RegisterModel
        {
            Email = email,
            Password = password
        };
        
        // Assert
        Assert.AreEqual(email, registerModel.Email);
        Assert.AreEqual(password, registerModel.Password);
    }

    [TestMethod]
    public void AfvalModel_Properties_SetAndGetCorrectly()
    {
        // Arrange
        var id = "123";
        var naam = "Plastic Container";
        var soort = "PMD";
        var datum = "2024-06-15";
        var tijd = "08:00";
        
        // Act
        var afvalModel = new AfvalModel
        {
            Id = id,
            Naam = naam,
            Soort = soort,
            Datum = datum,
            Tijd = tijd
        };
        
        // Assert
        Assert.AreEqual(id, afvalModel.Id);
        Assert.AreEqual(naam, afvalModel.Naam);
        Assert.AreEqual(soort, afvalModel.Soort);
        Assert.AreEqual(datum, afvalModel.Datum);
        Assert.AreEqual(tijd, afvalModel.Tijd);
    }

    [TestMethod]
    public void WeatherModel_Serialization_WorksCorrectly()
    {
        // Arrange
        var weatherJson = """
        {
            "latitude": 51.5,
            "longitude": 4.8,
            "generationtime_ms": 0.123,
            "utc_offset_seconds": 3600,
            "timezone": "Europe/Amsterdam",
            "timezone_abbreviation": "CET",
            "elevation": 10.0,
            "current_units": {
                "time": "iso8601",
                "interval": "seconds",
                "temperature_2m": "°C"
            },
            "current": {
                "time": "2024-06-15T08:00",
                "interval": 900,
                "temperature_2m": 20.5
            },
            "hourly_units": {
                "time": "iso8601",
                "temperature_2m": "°C",
                "rain": "mm"
            },
            "hourly": {
                "time": ["2024-06-15T08:00"],
                "temperature_2m": [20.5],
                "rain": [0.0]
            }
        }
        """;

        // Act
        var weatherModel = JsonSerializer.Deserialize<WeatherModel>(weatherJson);

        // Assert
        Assert.IsNotNull(weatherModel);
        Assert.AreEqual(51.5, weatherModel.Latitude);
        Assert.AreEqual(4.8, weatherModel.Longitude);
        Assert.AreEqual("Europe/Amsterdam", weatherModel.Timezone);
        Assert.IsNotNull(weatherModel.Current);
        Assert.AreEqual(20.5, weatherModel.Current.Temperature2m);
        Assert.IsNotNull(weatherModel.Hourly);
        Assert.AreEqual(1, weatherModel.Hourly.Time.Count);
        Assert.AreEqual("2024-06-15T08:00", weatherModel.Hourly.Time[0]);
    }

    [TestMethod]
    public void WeatherModel_EmptyConstructor_CreatesValidInstance()
    {
        // Act
        var weatherModel = new WeatherModel();

        // Assert
        Assert.IsNotNull(weatherModel);
        Assert.AreEqual(0, weatherModel.Latitude);
        Assert.AreEqual(0, weatherModel.Longitude);
    }

    [TestMethod]
    public void LoginModel_EmptyValues_HandledCorrectly()
    {
        // Act
        var loginModel = new LoginModel
        {
            Email = "",
            Password = ""
        };

        // Assert
        Assert.AreEqual("", loginModel.Email);
        Assert.AreEqual("", loginModel.Password);
    }

    [TestMethod]
    public void AfvalModel_NullValues_HandledCorrectly()
    {
        // Act
        var afvalModel = new AfvalModel
        {
            Id = null,
            Naam = null,
            Soort = null,
            Datum = null,
            Tijd = null
        };

        // Assert
        Assert.IsNull(afvalModel.Id);
        Assert.IsNull(afvalModel.Naam);
        Assert.IsNull(afvalModel.Soort);
        Assert.IsNull(afvalModel.Datum);
        Assert.IsNull(afvalModel.Tijd);
    }
}
