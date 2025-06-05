using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace frontendMonitoringUnittesten;

[TestClass]
public class ValidationTests
{
    [TestMethod]
    public void Email_Validation_ValidEmails_ReturnTrue()
    {
        // Arrange
        var validEmails = new[]
        {
            "test@example.com",
            "user.name@domain.co.uk",
            "user+tag@example.org",
            "firstname.lastname@example.com",
            "email@123.123.123.123", // IP address
            "1234567890@example.com"
        };

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        // Act & Assert
        foreach (var email in validEmails)
        {
            Assert.IsTrue(emailRegex.IsMatch(email), $"Email '{email}' should be valid");
        }
    }

    [TestMethod]
    public void Email_Validation_InvalidEmails_ReturnFalse()
    {
        // Arrange
        var invalidEmails = new[]
        {
            "invalid.email",
            "@missingdomain.com",
            "missing@.com",
            "spaces in@email.com",
            "email@",
            "@email.com",
            "email..double.dot@example.com",
            ""
        };

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        // Act & Assert
        foreach (var email in invalidEmails)
        {
            Assert.IsFalse(emailRegex.IsMatch(email), $"Email '{email}' should be invalid");
        }
    }

    [TestMethod]
    public void Password_Strength_ValidPasswords_ReturnTrue()
    {
        // Arrange
        var validPasswords = new[]
        {
            "Password123!",
            "MyStr0ngP@ssw0rd",
            "ComplexPass1!",
            "Secure123$",
            "Valid1Pass!"
        };

        // Password must have at least 8 characters, one uppercase, one lowercase, one digit, one special character
        var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");

        // Act & Assert
        foreach (var password in validPasswords)
        {
            Assert.IsTrue(passwordRegex.IsMatch(password), $"Password '{password}' should be valid");
        }
    }

    [TestMethod]
    public void Password_Strength_InvalidPasswords_ReturnFalse()
    {
        // Arrange
        var invalidPasswords = new[]
        {
            "password", // No uppercase, no digit, no special char
            "PASSWORD", // No lowercase, no digit, no special char
            "Password", // No digit, no special char
            "Password1", // No special char
            "Pass1!", // Too short
            "",
            "12345678", // Only digits
            "!@#$%^&*" // Only special characters
        };

        var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");

        // Act & Assert
        foreach (var password in invalidPasswords)
        {
            Assert.IsFalse(passwordRegex.IsMatch(password), $"Password '{password}' should be invalid");
        }
    }

    [TestMethod]
    public void JWT_Token_Format_ValidTokens_ReturnTrue()
    {
        // Arrange
        var validJwtTokens = new[]
        {
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.TJVA95OrM7E2cBab30RMHrHDcEfxjoYZgeFONFh7HgQ"
        };

        // JWT format: header.payload.signature (base64url encoded)
        var jwtRegex = new Regex(@"^[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+$");

        // Act & Assert
        foreach (var token in validJwtTokens)
        {
            Assert.IsTrue(jwtRegex.IsMatch(token), $"JWT token should be valid format");
        }
    }

    [TestMethod]
    public void JWT_Token_Format_InvalidTokens_ReturnFalse()
    {
        // Arrange
        var invalidJwtTokens = new[]
        {
            "invalid.token", // Only 2 parts
            "invalid.token.format.extra", // Too many parts
            "invalid-characters!.token.format", // Invalid characters
            "",
            "bearer-token-without-dots",
            "header.payload." // Missing signature
        };

        var jwtRegex = new Regex(@"^[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+$");

        // Act & Assert
        foreach (var token in invalidJwtTokens)
        {
            Assert.IsFalse(jwtRegex.IsMatch(token), $"JWT token '{token}' should be invalid format");
        }
    }

    [TestMethod]
    public void Date_Format_ValidDates_ReturnTrue()
    {
        // Arrange
        var validDates = new[]
        {
            "2024-06-15",
            "2023-12-31",
            "2025-01-01",
            "2024-02-29" // Leap year
        };

        // Act & Assert
        foreach (var dateString in validDates)
        {
            Assert.IsTrue(DateTime.TryParseExact(dateString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _), 
                         $"Date '{dateString}' should be valid");
        }
    }

    [TestMethod]
    public void Time_Format_ValidTimes_ReturnTrue()
    {
        // Arrange
        var validTimes = new[]
        {
            "08:00",
            "23:59",
            "00:00",
            "12:30"
        };

        // Act & Assert
        foreach (var timeString in validTimes)
        {
            Assert.IsTrue(TimeSpan.TryParseExact(timeString, @"hh\:mm", null, out _), 
                         $"Time '{timeString}' should be valid");
        }
    }
}
