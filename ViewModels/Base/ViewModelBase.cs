using CommunityToolkit.Mvvm.ComponentModel;

namespace NewLab.ViewModels.Base
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private bool isBusy;
    }
}
