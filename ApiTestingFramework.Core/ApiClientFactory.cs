namespace ApiTestingFramework.Core
{
    public static class ApiClientFactory
    {
        public static ApiClient CreateClient()
        {
            return ApiClient.Instance;
        }
    }
}