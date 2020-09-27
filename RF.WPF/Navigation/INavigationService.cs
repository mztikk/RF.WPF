using System.Collections.Generic;
using RF.WPF.MVVM;
using RF.WPF.UI.Interaction;

namespace RF.WPF.Navigation
{
    public interface INavigationService
    {
        ConfirmationResult GetConfirmation(string title, string message, IEnumerable<ConfirmationButtonInfo> buttons);
        void NavigateBack();
        void NavigateTo<VM>() where VM : ViewModelBase;
        void NavigateTo<VM>(VM viewModel) where VM : ViewModelBase;
    }
}
