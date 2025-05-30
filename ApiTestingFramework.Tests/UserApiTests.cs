using NUnit.Framework;
using RestSharp;
using ApiTestingFramework.Core;
using ApiTestingFramework.Business;
using ApiTestingFramework.Business.Models;
using System.Collections.Generic;
using System.Linq;
using RestSharp.Serializers.Json;

namespace ApiTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [Category("API")]
    public class UserApiTests : BaseTest
    {
        private readonly SystemTextJsonSerializer _serializer = new SystemTextJsonSerializer();

        [Test]
        public void Validate_GetUsers_ReturnsUserList()
        {
            Logger.Information("Starting test: Validate_GetUsers_ReturnsUserList");

            var request = new RequestBuilder("users", Method.Get).Build();
            var response = Client.Execute(request);
            var users = _serializer.Deserialize<List<User>>(response);

            Logger.Information("Validating response status code");
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Expected 200 OK status code");

            Logger.Information("Validating user list and properties");
            Assert.That(users, Is.Not.Null, "Response data should not be null");
            Assert.That(users.Count, Is.GreaterThan(0), "User list should not be empty");

            foreach (var user in users)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(user.Id, Is.Not.Null, "User ID should not be null");
                    Assert.That(user.Name, Is.Not.Null, "User Name should not be null");
                    Assert.That(user.Username, Is.Not.Null, "User Username should not be null");
                    Assert.That(user.Email, Is.Not.Null, "User Email should not be null");
                    Assert.That(user.Address, Is.Not.Null, "User Address should not be null");
                    Assert.That(user.Phone, Is.Not.Null, "User Phone should not be null");
                    Assert.That(user.Website, Is.Not.Null, "User Website should not be null");
                    Assert.That(user.Company, Is.Not.Null, "User Company should not be null");
                });
            }

            Logger.Information("Test completed successfully");
        }

        [Test]
        public void Validate_GetUsers_ResponseHeader()
        {
            Logger.Information("Starting test: Validate_GetUsers_ResponseHeader");

            var request = new RequestBuilder("users", Method.Get).Build();
            var response = Client.Execute(request);

            Logger.Information("Validating response status code");
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Expected 200 OK status code");

            Logger.Information("Validating Content-Type header");
            if (response.Headers == null || !response.Headers.Any())
            {
                Logger.Error("Headers collection is null or empty");
                Assert.Fail("Response headers are null or empty, cannot validate Content-Type");
            }

            // Log all headers for debugging
            Logger.Information("Response headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Name}: {h.Value}")));

            var contentType = response.Headers.FirstOrDefault(h => h.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))?.Value?.ToString();
            if (string.IsNullOrEmpty(contentType))
            {
                Logger.Warning("Content-Type header not found. Verifying response content is JSON.");
                // Check if the response content is valid JSON as a fallback
                try
                {
                    var jsonContent = System.Text.Json.JsonSerializer.Deserialize<object>(response.Content);
                    Logger.Information("Response content is valid JSON despite missing Content-Type header.");
                }
                catch (System.Text.Json.JsonException ex)
                {
                    Logger.Error("Content-Type header not found and response is not valid JSON: {Error}", ex.Message);
                    Assert.Fail($"Content-Type header not found and response is not valid JSON: {ex.Message}");
                }
            }
            else
            {
                Assert.That(contentType, Does.Contain("application/json"), "Expected Content-Type to contain 'application/json'");
                Logger.Information("Content-Type header validated: {ContentType}", contentType);
            }

            Logger.Information("Test completed successfully");
        }

        [Test]
        public void Validate_GetUsers_UsersCountAndProperties()
        {
            Logger.Information("Starting test: Validate_GetUsers_UsersCountAndProperties");

            var request = new RequestBuilder("users", Method.Get).Build();
            var response = Client.Execute(request);
            var users = _serializer.Deserialize<List<User>>(response);

            Logger.Information("Validating response status code");
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Expected 200 OK status code");

            Logger.Information("Validating user list count and properties");
            Assert.That(users.Count, Is.EqualTo(10), "Expected exactly 10 users");

            var userIds = users.Select(u => u.Id).Distinct().ToList();
            Assert.That(userIds.Count, Is.EqualTo(10), "All users should have unique IDs");

            foreach (var user in users)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(string.IsNullOrEmpty(user.Name), Is.False, $"User ID {user.Id} should have a non-empty Name");
                    Assert.That(string.IsNullOrEmpty(user.Username), Is.False, $"User ID {user.Id} should have a non-empty Username");
                    Assert.That(user.Company, Is.Not.Null, $"User ID {user.Id} should have a Company");
                    Assert.That(string.IsNullOrEmpty(user.Company.Name), Is.False, $"User ID {user.Id} should have a non-empty Company Name");
                });
            }

            Logger.Information("Test completed successfully");
        }

        [Test]
        public void Validate_CreateUser_Success()
        {
            Logger.Information("Starting test: Validate_CreateUser_Success");

            var newUser = new User { Name = "Test User", Username = "testuser" };
            var request = new RequestBuilder("users", Method.Post)
                .AddJsonBody(newUser)
                .Build();

            var response = Client.Execute(request);
            var createdUser = _serializer.Deserialize<User>(response);

            Logger.Information("Validating response status code");
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created), "Expected 201 Created status code");

            Logger.Information("Validating response content");
            Assert.That(createdUser, Is.Not.Null, "Response data should not be null");
            Assert.That(createdUser.Id, Is.Not.Null, "Created user should have an ID");

            Logger.Information("Test completed successfully");
        }

        [Test]
        public void Validate_GetInvalidEndpoint_ReturnsNotFound()
        {
            Logger.Information("Starting test: Validate_GetInvalidEndpoint_ReturnsNotFound");

            var request = new RequestBuilder("invalidendpoint", Method.Get).Build();
            var response = Client.Execute(request);

            Logger.Information("Validating response status code");
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound), "Expected 404 Not Found status code");

            Logger.Information("Test completed successfully");
        }
    }
}