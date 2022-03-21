using DMAdvantage.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace DMAdvantage.Client.Helpers
{
    public class AppRouteView : RouteView 
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        protected override void Render(RenderTreeBuilder builder)
        {
            var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
            if (authorize && !AccountService.IsLoggedIn())
            {
                NavigationManager.NavigateTo("account/login");
            }
            else
            {
                base.Render(builder);
            }
        }
    }
}
