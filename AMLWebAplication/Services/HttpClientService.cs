using Microsoft.IdentityModel.Tokens;
using System.Configuration;

namespace AMLWebAplication.Services
{
    public class HttpClientService
    {
        private readonly IConfiguration _configuration;
        public required HttpClient client { get; set; }

        public HttpClientService(IConfiguration configuration)
        {
            _configuration = configuration;

            ValidateBaseUris();
        }

        private void ValidateBaseUris()
        {
            if (string.IsNullOrWhiteSpace(_configuration["APIBaseURIs:AuthAPI"]))
                throw new Exception("AuthAPI base URI not found");

            if (string.IsNullOrWhiteSpace(_configuration["APIBaseURIs:UserAPI"]))
                throw new Exception("UserAPI base URI not found");

            if (string.IsNullOrWhiteSpace(_configuration["APIBaseURIs:InventoryAPI"]))
                throw new Exception("InventoryAPI base URI not found");

            if (string.IsNullOrWhiteSpace(_configuration["APIBaseURIs:ReportAPI"]))
                throw new Exception("ReportAPI base URI not found");
        }

        private HttpClient CreateHttpClient(string baseUri)
        {
            if (string.IsNullOrWhiteSpace(baseUri))
            {
                throw new ArgumentException("Base URI cannot be null or empty", nameof(baseUri));
            }

            return new HttpClient
            {
                BaseAddress = new Uri(baseUri)
            };
        }

        public HttpClient GetAuthHttpClient()
        {
            return CreateHttpClient(_configuration["APIBaseURIs:AuthAPI"]);
        }

        public HttpClient GetUserHttpClient()
        {
            return CreateHttpClient(_configuration["APIBaseURIs:UserAPI"]);
        }

        public HttpClient GetInventoryHttpClient()
        {
            return CreateHttpClient(_configuration["APIBaseURIs:InventoryAPI"]);
        }

        public HttpClient GetReportHttpClient()
        {
            return CreateHttpClient(_configuration["APIBaseURIs:ReportAPI"]);
        }
    }
}
