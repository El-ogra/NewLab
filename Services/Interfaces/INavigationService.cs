namespace NewLab.Services.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>() where TViewModel : class;
        void GoBack();
        bool CanGoBack { get; }
        object? CurrentViewModel { get; }
    }
}
