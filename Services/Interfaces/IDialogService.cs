namespace NewLab.Services.Interfaces
{
    public interface IDialogService
    {
        void ShowMessage(string title, string message);
        bool ShowConfirmation(string title, string message);
    }
}
