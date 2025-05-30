using NUnit.Framework;
using RestSharp;
using ApiTestingFramework.Core;
using ApiTestingFramework.Business;
using ApiTestingFramework.Business.Models;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using RestSharp.Serializers.Json;

namespace ApiTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [Category("API")]
    public class UserApiTests
    {
        private ApiClient _client;
        private ILogger _logger;
        private readonly SystemTextJsonSerializer _serializer = new SystemTextJsonSerializer();

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
            var response = _client.Execute(request);
            var users = _serializer.Deserialize<List<User>>(response);

            _logger.Information("Validating response status code");
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Expected 200 OK status code");

            _logger.Information("Validating user list and properties");
            Assert.That(users, Is.Not.Null, "Response data should not be null");
            Assert.That(users.Count, Is.GreaterThan(0), "User list should not be empty");

            foreach (var user in users)
            {
                Assert.That(user.Id, Is.Not.Null, "User ID should not be null");
                Assert.That(user.Name, Is.Not.Null, "User Name should not be null");
                Assert.That(user.Username, Is.Not.Null, "User Username should not be null");
                Assert.That(user.Email, Is.Not.Null, "User Email should not be null");
                Assert.That(user.Address, Is.Not.Null, "User Address should not be null");
                Assert.That(user.Phone, Is.Not.Null, "User Phone should not be null");
                Assert.That(user.Website, Is.Not.Null, "User Website should not be null");
                Assert.That(user.Company, Is.Not.Null, "User Company should not be null");
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
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Expected 200 OK status code");

            _logger.Information("Validating Content-Type header");
            if (response.Headers == null)
            {
                _logger.Error("Headers collection is null");
                Assert.Fail("Response headers are null, cannot validate Content-Type");
            }

            var contentType = response.Headers.FirstOrDefault(h => h.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))?.Value?.ToString();
            if (contentType == null)
            {
                _logger.Error("Content-Type header not found. Available headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Name}: {h.Value}")));
                Assert.Fail("Content-Type header not found in response");
            }

            Assert.That(contentType, Is.EqualTo("application/json; charset=utf-8"), "Unexpected Content-Type value");

            _logger.Information("Test completed successfully");
        }

        [Test]
        public void Validate_GetUsers_UsersCountAndProperties()
        {
            _logger.Information("Starting test: Validate_GetUsers_UsersCountAndProperties");

            var request = new RequestBuilder("users", Method.Get).Build();
            var response = _client.Execute(request);
            var users = _serializer.Deserialize<List<User>>(response);

            _logger.Information("Validating response status code");
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Expected 200 OK status code");

            _logger.Information("Validating user list count and properties");
            Assert.That(users.Count, Is.EqualTo(10), "Expected exactly 10 users");

            var userIds = users.Select(u => u.Id).Distinct().ToList();
            Assert.That(userIds.Count, Is.EqualTo(10), "All users should have unique IDs");

            foreach (var user in users)
            {
                Assert.That(string.IsNullOrEmpty(user.Name), Is.False, $"User ID {user.Id} should have a non-empty Name");
                Assert.That(string.IsNullOrEmpty(user.Username), Is.False, $"User ID {user.Id} should have a non-empty Username");
                Assert.That(user.Company, Is.Not.Null, $"User ID {user.Id} should have a Company");
                Assert.That(string.IsNullOrEmpty(user.Company.Name), Is.False, $"User ID {user.Id} should have a non-empty Company Name");
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

            var response = _client.Execute(request);
            var createdUser = _serializer.Deserialize<User>(response);

            _logger.Information("Validating response status code");
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created), "Expected 201 Created status code");

            _logger.Information("Validating response content");
            Assert.That(createdUser, Is.Not.Null, "Response data should not be null");
            Assert.That(createdUser.Id, Is.Not.Null, "Created user should have an ID");

            _logger.Information("Test completed successfully");
        }

        [Test]
        public void Validate_GetInvalidEndpoint_ReturnsNotFound()
        {
            _logger.Information("Starting test: Validate_GetInvalidEndpoint_ReturnsNotFound");

            var request = new RequestBuilder("invalidendpoint", Method.Get).Build();
            var response = _client.Execute(request);

            _logger.Information("Validating response status code");
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound), "Expected 404 Not Found status code");

            _logger.Information("Test completed successfully");
        }
    }
}