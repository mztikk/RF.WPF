using RF.WPF.MVVM;

namespace RF.WPF.Navigation
{
    public interface INavigationService
    {
        void NavigateBack();
        void NavigateTo<VM>() where VM : ViewModelBase;
        void NavigateTo<VM>(VM viewModel) where VM : ViewModelBase;
    }
}
