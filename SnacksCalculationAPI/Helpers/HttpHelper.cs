
namespace SnacksCalculationAPI.Helpers
{
    public  class HttpHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public HttpContext HttpContext => _httpContextAccessor.HttpContext;
    }
}
