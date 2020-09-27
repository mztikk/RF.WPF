using System.Collections.Generic;
using System.Threading.Tasks;
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

        private readonly Stack<ViewModelBase> _stack;

        public NavigationService(IContainer container, IWindowManager windowManager)
        {
            _stack = new Stack<ViewModelBase>();

            _container = container;
            _windowManager = windowManager;

            // poll mainwindow until i know something better
            Execute.PostToUIThreadAsync(async () =>
            {
                while (Application.Current.MainWindow is null)
                {
                    await Task.Delay(15);
                }

                if (Application.Current.MainWindow.DataContext is ViewModelBase vm)
                {
                    _stack.Push(vm);
                }
            });
        }

        private ViewModelBase GetLastViewModel() => _stack.Peek();

        public void NavigateTo<VM>() where VM : ViewModelBase
        {
            VM viewModel = _container.Get<VM>();
            InternalNavigateTo(viewModel);
        }

        public void NavigateTo<VM>(VM viewModel) where VM : ViewModelBase => NavigateTo(viewModel, null);
        public void NavigateTo<VM>(VM viewModel, IViewAware? owner) where VM : ViewModelBase => InternalNavigateTo(viewModel, owner);

        public void NavigateBack()
        {
            if (_stack.Count > 0)
            {
                _stack.Pop().RequestClose();
            }
            else
            {
                Application.Current.Dispatcher.InvokeShutdown();
            }
        }

        public ConfirmationResult GetConfirmation(string title, string message, IEnumerable<ConfirmationButtonInfo> buttons)
        {
            ConfirmationViewModel vm = _container.Get<ConfirmationViewModel>();
            vm.Title = title;
            vm.Message = message;
            vm.Buttons.AddRange(buttons);
            NavigateTo(vm, GetLastViewModel());
            return vm.ConfirmationResult;
        }

        private void InternalNavigateTo<VM>(VM viewModel, IViewAware? owner = null) where VM : ViewModelBase
        {
            _stack.Push(viewModel);
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
