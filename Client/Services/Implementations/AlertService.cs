using DMAdvantage.Client.Models;
using System;

namespace DMAdvantage.Client.Services.Implementations
{
    public class AlertService : IAlertService
    {
        private const string _defaultId = "default-alert";
        public event Action<Alert>? OnAlert;

        public void Alert(AlertType type, string message, bool keepAfterRouteChange = false, bool autoClose = true)
        {
            Alert(new Alert
            {
                Type = type,
                Message = message,
                KeepAfterRouteChange = keepAfterRouteChange,
                AutoClose = autoClose
            });
        }

        public void Alert(Alert alert)
        {
            alert.Id ??= _defaultId;
            OnAlert?.Invoke(alert);
        }

        public void Clear(string? id = _defaultId)
        {
            OnAlert?.Invoke(new Alert { Id = id });
        }
    }
}
