using System.Windows;
using RF.WPF.MVVM;
using RF.WPF.UI.Interaction;
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

        public void NavigateTo<VM>() where VM : ViewModelBase => NavigateTo<VM>(null);

        public void NavigateTo<VM>(IViewAware? owner) where VM : ViewModelBase
        {
            VM viewModel = _container.Get<VM>();
            InternalNavigateTo(viewModel, owner);
        }

        public void NavigateTo<VM>(VM viewModel) where VM : ViewModelBase => NavigateTo(viewModel, null);
        public void NavigateTo<VM>(VM viewModel, IViewAware? owner) where VM : ViewModelBase => InternalNavigateTo(viewModel, owner);

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

        public ConfirmationResult GetConfirmation(string title, string message, string affirmativeText = "Yes", string negativeText = "No", string cancelText = "Cancel")
        {
            ConfirmationViewModel vm = _container.Get<ConfirmationViewModel>();
            vm.Title = title;
            vm.Message = message;
            vm.AffirmativeText = affirmativeText;
            vm.NegativeText = negativeText;
            vm.CancelText = cancelText;
            NavigateTo(vm);
            return vm.ConfirmationResult;
        }

        private void InternalNavigateTo<VM>(VM viewModel, IViewAware? owner = null) where VM : ViewModelBase
        {
            _lastViewModel = viewModel;
            viewModel.OnNavigatedTo();

            if (owner is { })
            {
                _windowManager.ShowDialog(viewModel, owner);
            }
            else
            {
                _windowManager.ShowDialog(viewModel);
            }
        }
    }
}
