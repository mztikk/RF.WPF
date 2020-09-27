using System.Windows;
using RF.WPF.MVVM;

namespace RF.WPF.UI.Interaction
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ConfirmationView : ViewBase<ConfirmationViewModel>
    {
        public ConfirmationView(ConfirmationViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
