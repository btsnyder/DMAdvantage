using DMAdvantage.Client.Services;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Shared
{
    public abstract class BaseEditForm<T> : ComponentBase where T : class, new()
    {
        protected T _model = new();
        protected MudForm _form;
        protected bool _loading = true;
        protected BaseValidator<T> _validator;

        [Inject] protected IApiService ApiService { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        [Parameter] public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<T>(Guid.Parse(Id)) ?? new T();
            }

            await base.OnInitializedAsync();
            _loading = false;
        }

        private async void OnValidSubmit()
        {
            _loading = true;
            try
            {
                if (Id == null)
                {
                    await ApiService.AddEntity(_model);
                    Snackbar.Add($"{typeof(T).Name} added successfully", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    Snackbar.Add("Update successful", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                NavigationManager.NavigateTo(GenericHelpers.GetPath<T>());
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error submitting change: {ex}", Severity.Error);
            }
            _loading = false;
        }

        protected async Task OnSubmit()
        {
            _validator.Snackbar = Snackbar;
            await _form.Validate();
            _validator.Snackbar = null;

            if (_form.IsValid)
            {
                OnValidSubmit();
            }
        }
    }
}
