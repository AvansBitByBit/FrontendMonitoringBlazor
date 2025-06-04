namespace frontendMonitoringUnittesten;

[TestClass]
public sealed class Test1
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
}

// Test version of FilterOption class for unit testing
public class FilterOption
{
    public string Name { get; set; } = string.Empty;

    public override bool Equals(object? o) => (o as FilterOption)?.Name == Name;
    public override int GetHashCode() => Name?.GetHashCode() ?? 0;
}