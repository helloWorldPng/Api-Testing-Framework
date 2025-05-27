using NUnit.Framework;
using RestSharp;
using ApiTestingFramework.Core;
using ApiTestingFramework.Business;
using ApiTestingFramework.Business.Models;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ApiTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [Category("API")]
    public class UserApiTests
    {
        private ApiClient _client;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _client = new ApiClient();
            _logger = Log.ForContext<UserApiTests>();
        }

        [Test]
        public void Validate_GetUsers_ReturnsUserList()
        {
            _logger.Information("Starting test: Validate_GetUsers_ReturnsUserList");

            var request = new RequestBuilder("users", Method.Get).Build();
            var response = _client.Execute<List<User>>(request);

            _logger.Information("Validating response status code");
            Assert.AreEqual(200, (int)response.StatusCode, "Expected 200 OK status code");

            _logger.Information("Validating user list and properties");
            Assert.IsNotNull(response, "Response data should not be null");
            Assert.IsTrue(response.Count > 0, "User list should not be empty");

            foreach (var user in response)
            {
                Assert.IsNotNull(user.Id, "User ID should not be null");
                Assert.IsNotNull(user.Name, "User Name should not be null");
                Assert.IsNotNull(user.Username, "User Username should not be null");
                Assert.IsNotNull(user.Email, "User Email should not be null");
                Assert.IsNotNull(user.Address, "User Address should not be null");
                Assert.IsNotNull(user.Phone, "User Phone should not be null");
                Assert.IsNotNull(user.Website, "User Website should not be null");
                Assert.IsNotNull(user.Company, "User Company should not be null");
            }

            _logger.Information("Test completed successfully");
        }

        [Test]
        public void Validate_GetUsers_ResponseHeader()
        {
            _logger.Information("Starting test: Validate_GetUsers_ResponseHeader");

            var request = new RequestBuilder("users", Method.Get).Build();
            var response = _client.Execute(request);

            _logger.Information("Validating response status code");
            Assert.AreEqual(200, (int)response.StatusCode, "Expected 200 OK status code");

            _logger.Information("Validating Content-Type header");
            var contentType = response.Headers.FirstOrDefault(h => h.Name.Equals("Content-Type"))?.Value?.ToString();
            Assert.IsNotNull(contentType, "Content-Type header should exist");
            Assert.AreEqual("application/json; charset=utf-8", contentType, "Unexpected Content-Type value");

            _logger.Information("Test completed successfully");
        }

        [Test]
        public void Validate_GetUsers_UsersCountAndProperties()
        {
            _logger.Information("Starting test: Validate_GetUsers_UsersCountAndProperties");

            var request = new RequestBuilder("users", Method.Get).Build();
            var response = _client.Execute<List<User>>(request);

            _logger.Information("Validating response status code");
            Assert.AreEqual(200, (int)response.StatusCode, "Expected 200 OK status code");

            _logger.Information("Validating user list count and properties");
            Assert.AreEqual(10, response.Count, "Expected exactly 10 users");

            var userIds = response.Select(u => u.Id).Distinct().ToList();
            Assert.AreEqual(10, userIds.Count, "All users should have unique IDs");

            foreach (var user in response)
            {
                Assert.IsFalse(string.IsNullOrEmpty(user.Name), $"User ID {user.Id} should have a non-empty Name");
                Assert.IsFalse(string.IsNullOrEmpty(user.Username), $"User ID {user.Id} should have a non-empty Username");
                Assert.IsNotNull(user.Company, $"User ID {user.Id} should have a Company");
                Assert.IsFalse(string.IsNullOrEmpty(user.Company.Name), $"User ID {user.Id} should have a non-empty Company Name");
            }

            _logger.Information("Test completed successfully");
        }

        [Test]
        public void Validate_CreateUser_Success()
        {
            _logger.Information("Starting test: Validate_CreateUser_Success");

            var newUser = new User { Name = "Test User", Username = "testuser" };
            var request = new RequestBuilder("users", Method.Post)
                .AddJsonBody(newUser)
                .Build();

            var response = _client.Execute<User>(request);

            _logger.Information("Validating response status code");
            Assert.AreEqual(201, (int)response.StatusCode, "Expected 201 Created status code");

            _logger.Information("Validating response content");
            Assert.IsNotNull(response, "Response data should not be null");
            Assert.IsNotNull(response.Id, "Created user should have an ID");

            _logger.Information("Test completed successfully");
        }

        [Test]
        public void Validate_GetInvalidEndpoint_ReturnsNotFound()
        {
            _logger.Information("Starting test: Validate_GetInvalidEndpoint_ReturnsNotFound");

            var request = new RequestBuilder("invalidendpoint", Method.Get).Build();
            var response = _client.Execute(request);

            _logger.Information("Validating response status code");
            Assert.AreEqual(404, (int)response.StatusCode, "Expected 404 Not Found status code");

            _logger.Information("Test completed successfully");
        }
    }
}

