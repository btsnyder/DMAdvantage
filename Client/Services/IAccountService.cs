using DMAdvantage.Shared.Models;
using System.Threading.Tasks;

namespace DMAdvantage.Client.Services
{
    public interface IAccountService
    {
        LoginResponse? User { get; }
        Task Initialize();
        Task Login(LoginRequest model);
        Task Logout();
        public bool GetLoggedIn();
    }
}
