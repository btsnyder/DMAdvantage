namespace DMAdvantage.Client.Services.Implementations
{
    public class ApiService : IApiService
    {
        private readonly IHttpService _httpService;

        public ApiService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task AddEntity<T>(T model)
        {
            if (model == null)
                return;
            await _httpService.Post($"/api/{GetPath(typeof(T))}", model);
        }

        public async Task<T?> GetEntityById<T>(Guid id)
        {
            return await _httpService.Get<T>($"/api/{GetPath(typeof(T))}/{id}");
        }

        public async Task UpdateEntity<T>(Guid id, T model)
        {
            if (model == null)
                return;
            await _httpService.Put($"/api/{GetPath(typeof(T))}/{id}", model);
        }

        public async Task<List<T>?> GetAllEntities<T>()
        {
            return await _httpService.Get<List<T>>($"/api/{GetPath(typeof(T))}");
        }

        public async Task RemoveEntity<T>(Guid id)
        {
            await _httpService.Delete($"/api/{GetPath(typeof(T))}/{id}");
        }

        private static string GetPath(Type t)
        {
            var path = t.Name;
            path = path.Replace("Request", "");
            path = path.Replace("Response", "");
            path += "s";
            return path.ToLower();
        }
    }
}
