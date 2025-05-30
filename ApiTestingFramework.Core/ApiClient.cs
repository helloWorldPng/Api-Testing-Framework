using Microsoft.Extensions.Configuration;
using RestSharp;
using Serilog;
using Serilog.Settings.Configuration;
using System;

namespace ApiTestingFramework.Core
{
    public class ApiClient
    {
        private readonly RestClient _client;
        private readonly ILogger _logger;

        public ApiClient()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var baseUrl = configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("BaseUrl is not configured in appsettings.json");
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .WriteTo.File("logs/apitesting.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _logger = Log.ForContext<ApiClient>();
            _logger.Information("Initializing ApiClient with base URL: {BaseUrl}", baseUrl);

            _client = new RestClient(baseUrl);
        }

        public RestResponse Execute(RestRequest request)
        {
            _logger.Information("Executing request: {Method} {Resource}", request.Method, request.Resource);
            var response = _client.Execute(request);
            _logger.Information("Received response: StatusCode={StatusCode}, ContentLength={ContentLength}",
                response.StatusCode, response.Content?.Length);
            return response;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            _logger.Information("Executing request: {Method} {Resource}", request.Method, request.Resource);
            var response = _client.Execute<T>(request);
            _logger.Information("Received response: StatusCode={StatusCode}, ContentLength={ContentLength}",
                response.StatusCode, response.Content?.Length);
            return response.Data ?? new T();
        }
    }
}