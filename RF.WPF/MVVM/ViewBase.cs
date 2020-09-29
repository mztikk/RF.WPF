using System.Windows;

namespace RF.WPF.MVVM
{
    public class ViewBase<VM> : Window where VM : ViewModelBase
    {

        public readonly VM ViewModel;

        //public ViewBase(VM viewModel) => ViewModel = viewModel;
    }
}
