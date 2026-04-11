using System.Net.Http.Headers;

namespace lms.web.Services
{
    /// <summary>
    /// Delegating handler that reads the JWT stored in the user's session
    /// and attaches it as a Bearer token to every outbound microservice call.
    /// </summary>
    public class JwtForwardingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtForwardingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
