using Microsoft.AspNetCore.Components;
using DMAdvantage.Shared.Models;

namespace DMAdvantage.Client.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IHttpService _httpService;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorageService;
        public static readonly string UserKey = "user";
        public LoginResponse? User { get; private set; }

        public AccountService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService)
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<LoginResponse>(UserKey);
            if (User == null) return;
            User = await _httpService.Get<LoginResponse>("/api/account/refresh");
            await _localStorageService.SetItem(UserKey, User);
        }

        public async Task Login(LoginRequest request)
        {
            User = await _httpService.Post<LoginResponse>("/api/account/token", request);
            await _localStorageService.SetItem(UserKey, User);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem(UserKey);
            _navigationManager.NavigateTo("account/login");
        }

        public bool IsLoggedIn()
        {
            return User != null;
        }
    }
}
