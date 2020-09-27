using RF.WPF.MVVM;
using RF.WPF.Navigation;

namespace RF.WPF.UI.Interaction
{
    public class ConfirmationViewModel : ViewModelBase
    {
        public ConfirmationViewModel(INavigationService navigationService) : base(navigationService) => ConfirmationResult = ConfirmationResult.None;


        public override void OnNavigatedTo()
        {
            ConfirmationResult = ConfirmationResult.None;

            base.OnNavigatedTo();
        }


        private string _message;
        public string Message
        {
            get => _message;
            set { _message = value; NotifyOfPropertyChange(); }
        }


        private string _affirmativeText;
        public string AffirmativeText
        {
            get => _affirmativeText;
            set { _affirmativeText = value; NotifyOfPropertyChange(); }
        }


        private string _negativeText;
        public string NegativeText
        {
            get => _negativeText;
            set { _negativeText = value; NotifyOfPropertyChange(); }
        }


        private string _cancelText;
        public string CancelText
        {
            get => _cancelText;
            set { _cancelText = value; NotifyOfPropertyChange(); }
        }


        private ConfirmationResult _confirmationResult;
        public ConfirmationResult ConfirmationResult
        {
            get => _confirmationResult;
            set { _confirmationResult = value; NotifyOfPropertyChange(); }
        }

        public void Affirmative()
        {
            ConfirmationResult = ConfirmationResult.Affirmative;
            NavigateBack();
        }

        public void Negative()
        {
            ConfirmationResult = ConfirmationResult.Negative;
            NavigateBack();
        }

        public void Cancel()
        {
            ConfirmationResult = ConfirmationResult.Cancel;
            NavigateBack();
        }
    }
}
