using FrontendMonitoring.Models;
using HttpRequester;

namespace FrontendMonitoring.Services

{
    public class AfvalApiClient
    {
        private readonly HttpRequester.HttpRequester _httpRequester;

        public AfvalApiClient(HttpRequester.HttpRequester httpRequester)
        {
            _httpRequester = httpRequester;
        }

        public Task<AfvalModel?> GetDataAsync()
        {
            return _httpRequester.GetAsync<AfvalModel>("api/Dummytrash");
        }
    }
}