using RF.WPF.MVVM;
using RF.WPF.UI.Interaction;

namespace RF.WPF.Navigation
{
    public interface INavigationService
    {
        ConfirmationResult GetConfirmation(string title, string message, string affirmativeText = "Yes", string negativeText = "No", string cancelText = "Cancel");
        void NavigateBack();
        void NavigateTo<VM>() where VM : ViewModelBase;
        void NavigateTo<VM>(VM viewModel) where VM : ViewModelBase;
    }
}
