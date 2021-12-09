using DMAdvantage.Client.Models;

namespace DMAdvantage.Client.Services
{
    public interface IAlertService
    {
        event Action<Alert> OnAlert;
        void Alert(AlertType type, string message, bool keepAfterRouteChange = false, bool autoClose = true);
        void Alert(Alert alert);
        void Clear(string? id = null);
    }
}
