namespace MyAcademy_ApiWebUIProject.Services
{
    // ApiService'in HttpClient'ına eklenen ara katman: API'ye hiç ulaşılamazsa (API kapalı)
    // isteğe bir işaret bırakır, layout bu işarete bakıp sayfanın üstünde uyarı gösterir.
    public class ApiAvailabilityHandler(IHttpContextAccessor _httpContextAccessor) : DelegatingHandler
    {
        public const string ApiDownKey = "ApiDown";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (HttpRequestException)
            {
                if (_httpContextAccessor.HttpContext != null)
                    _httpContextAccessor.HttpContext.Items[ApiDownKey] = true;
                throw; // ApiService yine kendi catch bloğunda boş sonuç döner
            }
        }
    }
}
