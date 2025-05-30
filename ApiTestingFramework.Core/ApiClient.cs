using Microsoft.Extensions.Configuration;
using RestSharp;
using Serilog;
using System;

namespace ApiTestingFramework.Core
{
    public class ApiClient
    {
        private static ApiClient _instance;
        private static readonly object _lock = new object();
        private readonly RestClient _client;
        private readonly ILogger _logger;

        private ApiClient(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("BaseUrl is not configured in appsettings.json");
            }

            _logger = Log.ForContext<ApiClient>();
            _logger.Information("Initializing ApiClient with base URL: {BaseUrl}", baseUrl);

            _client = new RestClient(baseUrl);
        }

        public static ApiClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            try
                            {
                                var configuration = new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                    .Build();

                                // Fallback to a default base URL if configuration is missing
                                string baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://jsonplaceholder.typicode.com";

                                // Initialize Serilog with a minimal configuration if file-based config fails
                                Log.Logger = new LoggerConfiguration()
                                    .MinimumLevel.Information()
                                    .WriteTo.Console()
                                    .WriteTo.File("logs/apitesting_fallback.log", rollingInterval: RollingInterval.Day)
                                    .CreateLogger();

                                _instance = new ApiClient(baseUrl);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to initialize ApiClient: {ex.Message}");
                                throw;
                            }
                        }
                    }
                }
                return _instance;
            }
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