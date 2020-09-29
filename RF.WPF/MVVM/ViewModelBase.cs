using RF.WPF.Navigation;
using Stylet;

namespace RF.WPF.MVVM
{
    public class ViewModelBase : Screen
    {
        protected readonly INavigationService _navigationService;

        public ViewModelBase(INavigationService navigationService)
        {
            Title = GetType().Name;
            _navigationService = navigationService;
        }

        private string _title;

        public string Title
        {
            get => _title;
            set { _title = value; NotifyOfPropertyChange(); }
        }

        public virtual void OnNavigatedTo() { }

        public virtual void NavigateBack() => _navigationService.NavigateBack();
    }
}
