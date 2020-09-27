using RF.WPF.MVVM;
using RF.WPF.Navigation;
using Stylet;

namespace RF.WPF.UI.Interaction
{
    public class ConfirmationViewModel : ViewModelBase
    {
        public ConfirmationViewModel(INavigationService navigationService) : base(navigationService)
        {
            ConfirmationResult = ConfirmationResult.None;
            Buttons = new BindableCollection<ConfirmationButtonInfo>();
        }

        public override void OnNavigatedTo()
        {
            ConfirmationResult = ConfirmationResult.None;

            base.OnNavigatedTo();
        }

        private BindableCollection<ConfirmationButtonInfo> _buttons;
        public BindableCollection<ConfirmationButtonInfo> Buttons
        {
            get => _buttons;
            set { _buttons = value; NotifyOfPropertyChange(); }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set { _message = value; NotifyOfPropertyChange(); }
        }

        private ConfirmationResult _confirmationResult;
        public ConfirmationResult ConfirmationResult
        {
            get => _confirmationResult;
            set { _confirmationResult = value; NotifyOfPropertyChange(); }
        }

        public void OnButton(ConfirmationButtonInfo button)
        {
            ConfirmationResult = button.Type;
            Buttons.Clear();
            NavigateBack();
        }
    }
}
