using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using System.Text.Json;

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

        public async Task<T?> GetEntityById<T>(Guid id) where T : class
        {
            return await _httpService.Get<T>($"/api/{GetPath(typeof(T))}/{id}");
        }

        public async Task UpdateEntity<T>(Guid id, T model)
        {
            if (model == null)
                return;
            await _httpService.Put($"/api/{GetPath(typeof(T))}/{id}", model);
        }

        public async Task<List<T>?> GetAllEntities<T>(ISearchQuery? searching = null) where T : class
        {
            var data = new List<T>();
            PagedList<T>? paged;
            int pageNumber = 1;
            do
            {
                paged = await GetAllPagedEntities<T>(new PagingParameters { PageNumber = pageNumber, PageSize = 100 }, searching);
                if (paged != null)
                {
                    data.AddRange(paged);
                    pageNumber = paged.CurrentPage + 1;
                }
            } while (paged?.HasNext == true);
            return data;
        }

        public async Task<PagedList<T>?> GetAllPagedEntities<T>(PagingParameters paging, ISearchQuery? searching = null, CancellationToken? token = null) where T : class
        {
            var uri = $"/api/{GetPath(typeof(T))}?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}";
            if (searching != null)
                uri += $"&{searching.GetQuery()}";
            var (data, headers) = await _httpService.GetWithResponseHeader<List<T>>(uri, token);

            if (data == null)
                return null;

            PagedData? pagingResponse = null;
            if (headers?.TryGetValues(PagedData.Header.ToLower(), out IEnumerable<string>? values) == true)
            {
                var pagingHeader = values?.FirstOrDefault();
                if (pagingHeader != null)
                    pagingResponse = JsonSerializer.Deserialize<PagedData>(pagingHeader);
            }

            pagingResponse ??= new PagedData
            {
                TotalCount = data.Count,
                PageSize = paging.PageSize,
                CurrentPage = paging.PageNumber,
                TotalPages = 1
            };

            return new PagedList<T>(data, pagingResponse.TotalCount, pagingResponse.CurrentPage, pagingResponse.PageSize);
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

        public async Task<EncounterResponse?> GetEncounterView(Guid id)
        {
            return await _httpService.Get<EncounterResponse>($"/api/view/encounter/{id}");
        }

        public async Task<List<T>?> GetViews<T>(IEnumerable<Guid>? ids = null)
        {
            var uri = $"/api/view/{GetPath(typeof(T))}";
            var values = ids?.ToList();
            if (values != null && values.Any())
            {
                uri += "?";
                uri = values.Aggregate(uri, (current, id) => current + $"ids={id}&");
            }
            uri = uri.TrimEnd('&');
            return await _httpService.Get<List<T>>(uri);
        }

        public async Task<CharacterResponse?> GetCharacterViewFromPlayerName(string name)
        {
            var uri = $"/api/view/characters/player/{name}";
            return await _httpService.Get<CharacterResponse>(uri);
        }
    }
}
