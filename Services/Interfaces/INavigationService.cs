namespace NewLab.Services.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>() where TViewModel : ViewModels.Base.ViewModelBase;
        void GoBack();
        bool CanGoBack { get; }
    }
}
