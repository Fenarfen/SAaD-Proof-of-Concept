using InventoryAPI.Services;

namespace InventoryAPI.Tests;

[TestFixture]
public class AuthenticationService_SatisfiesInterface
{
    private const string _exampleVaildToken = "Lbz6cmmtGGjpVXhnQUuKYQo9Q4S1rUSgt4yBeniUJmS7BsrrjtVDk3bzS3qEbVED";
    private const string _exampleInvalidToken = "gr()h73+..";

    private AuthenticationService _authenticationService;
    private HttpRequest _exampleVaildRequest;
    private HttpRequest _exampleNoAuthHeaderRequest;
    private HttpRequest _exampleInvalidAuthHeaderRequest;

    [SetUp]
    public void Setup()
    {
        _authenticationService = new AuthenticationService();

        // New up instance of fake http request
        var httpContext = new DefaultHttpContext();
        _exampleNoAuthHeaderRequest = httpContext.Request;

        httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = $"Bea {_exampleVaildToken}";
        _exampleInvalidAuthHeaderRequest = httpContext.Request;

        httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = $"Bearer {_exampleVaildToken}";
        _exampleVaildRequest = httpContext.Request;
    }

    [Test]
    public void GetToken_VaildToken()
    {
        var actual = _authenticationService.GetToken(_exampleVaildRequest);

        Assert.That(actual, Is.EqualTo(_exampleVaildToken), "Token was retrived successfully");
    }

    [Test]
    public void GetToken_NoAuthHeader()
    {
        var actual = _authenticationService.GetToken(_exampleNoAuthHeaderRequest);

        Assert.That(actual, Is.EqualTo(""), "Token was unexpectedly retrived");
    }

    [Test]
    public void GetToken_InvalidAuthHeader()
    {
        var actual = _authenticationService.GetToken(_exampleInvalidAuthHeaderRequest);

        Assert.That(actual, Is.EqualTo(""), "Token was unexpectedly retrived");
    }

    [Test]
    public async Task GetAccount_ValidToken()
    {
        var actual = await _authenticationService.GetAccount(_exampleVaildToken);
        
        Assert.Multiple(() =>
        {
            Assert.That(actual.Role.Name, Is.EqualTo("Manager"), "Unexpected role was returned");
            Assert.That(actual.Address.City, Is.EqualTo("Sheffield"), "Unexpected city was returned");
        });
    }

    [Test]
    public async Task GetAccount_InvalidToken()
    {
        var actual = await _authenticationService.GetAccount(_exampleInvalidToken);

        Assert.That(actual, Is.Null);
    }
}