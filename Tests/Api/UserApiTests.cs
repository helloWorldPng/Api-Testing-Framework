using NUnit.Framework;
using RestSharp;
using ApiTestingFramework.Core;
using ApiTestingFramework.Business;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ApiTestingFramework.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [Category("API")]
    public class UserApiTests : BaseTest
    {
        private ApiClient _client;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _client = new ApiClient(Configuration);
            _logger = Logger.ForContext<UserApiTests>();
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
    }
}