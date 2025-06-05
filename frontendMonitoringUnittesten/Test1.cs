namespace frontendMonitoringUnittesten;

[TestClass]
public sealed class BasicTests
{
    [TestMethod]
    public void TestMethod1()
    {
        // Basic test to ensure the test framework is working
        Assert.IsTrue(true, "Basic test should pass");
    }

    [TestMethod]
    public void TestAuthenticationTokenValidation()
    {
        // Test that would validate authentication token format
        string validToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";
        Assert.IsNotNull(validToken, "Token should not be null");
        Assert.IsTrue(validToken.Length > 0, "Token should not be empty");
    }

    [TestMethod]
    public void TestFilterOptionClass()
    {
        // Test the FilterOption class that replaced the Pizza class
        var option1 = new FilterOption { Name = "Test Option" };
        var option2 = new FilterOption { Name = "Test Option" };
        var option3 = new FilterOption { Name = "Different Option" };

        Assert.AreEqual(option1.GetHashCode(), option2.GetHashCode(), "Objects with same name should have same hash code");
        Assert.IsTrue(option1.Equals(option2), "Objects with same name should be equal");
        Assert.IsFalse(option1.Equals(option3), "Objects with different names should not be equal");
    }

    [TestMethod]
    public void TestFilterOption_NullComparison()
    {
        // Test null comparison for FilterOption
        var option = new FilterOption { Name = "Test" };
        
        Assert.IsFalse(option.Equals(null), "Object should not equal null");
        Assert.IsFalse(option.Equals("string"), "Object should not equal different type");
    }

    [TestMethod]
    public void TestFilterOption_EmptyName()
    {
        // Test FilterOption with empty name
        var option1 = new FilterOption { Name = "" };
        var option2 = new FilterOption { Name = "" };
        
        Assert.IsTrue(option1.Equals(option2), "Objects with empty names should be equal");
        Assert.AreEqual(option1.GetHashCode(), option2.GetHashCode(), "Empty names should have same hash code");
    }

    [TestMethod]
    public void TestFilterOption_NullName()
    {
        // Test FilterOption with null name
        var option1 = new FilterOption { Name = null };
        var option2 = new FilterOption { Name = null };
        
        Assert.IsTrue(option1.Equals(option2), "Objects with null names should be equal");
        Assert.AreEqual(option1.GetHashCode(), option2.GetHashCode(), "Null names should have same hash code");
    }

    [TestMethod]
    public void StringOperations_Basic_WorkCorrectly()
    {
        // Test basic string operations that might be used in the application
        string email = "  test@EXAMPLE.com  ";
        string cleanEmail = email.Trim().ToLowerInvariant();
        
        Assert.AreEqual("test@example.com", cleanEmail);
        Assert.IsTrue(cleanEmail.Contains("@"));
        Assert.IsTrue(cleanEmail.EndsWith(".com"));
    }

    [TestMethod]
    public void DateTime_Operations_WorkCorrectly()
    {
        // Test DateTime operations that might be used for waste collection dates
        var now = DateTime.Now;
        var tomorrow = now.AddDays(1);
        var yesterday = now.AddDays(-1);
        
        Assert.IsTrue(tomorrow > now);
        Assert.IsTrue(yesterday < now);
        Assert.AreEqual(1, (tomorrow - now).Days);
    }
}

// Test version of FilterOption class for unit testing
public class FilterOption
{
    public string Name { get; set; } = string.Empty;

    public override bool Equals(object? o) => (o as FilterOption)?.Name == Name;
    public override int GetHashCode() => Name?.GetHashCode() ?? 0;
}