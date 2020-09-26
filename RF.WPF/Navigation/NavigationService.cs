using System.Windows;
using RF.WPF.MVVM;
using Stylet;
using StyletIoC;

namespace RF.WPF.Navigation
{
    internal class NavigationService : INavigationService
    {
        private readonly IContainer _container;
        private readonly IWindowManager _windowManager;

        public NavigationService(IContainer container, IWindowManager windowManager)
        {
            _container = container;
            _windowManager = windowManager;
        }

        private ViewModelBase? _lastViewModel = null;

        public void NavigateTo<VM>() where VM : ViewModelBase
        {
            VM viewModel = _container.Get<VM>();
            InternalNavigateTo(viewModel);
        }

        public void NavigateTo<VM>(VM viewModel) where VM : ViewModelBase => InternalNavigateTo(viewModel);

        public void NavigateBack()
        {
            if (_lastViewModel is { })
            {
                _lastViewModel.RequestClose();
            }
            else
            {
                Application.Current.Dispatcher.InvokeShutdown();
            }
        }

        private void InternalNavigateTo<VM>(VM viewModel) where VM : ViewModelBase
        {
            _lastViewModel = viewModel;
            viewModel.OnNavigatedTo();
            _windowManager.ShowDialog(viewModel);
        }
    }
}
