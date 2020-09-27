using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Stylet;

namespace RF.WPF.UI.Interaction
{
    public class ConfirmationButtonInfo : PropertyChangedBase
    {
        public ConfirmationButtonInfo(ConfirmationResult type, string text)
        {
            object textColorResource = Application.Current.TryFindResource("TextColor");
            if (textColorResource is { })
            {
                TextColor = new SolidColorBrush((Color)textColorResource);
                TextColor.Freeze();
            }

            object accentColorResource = Application.Current.TryFindResource("AccentColor");
            if (accentColorResource is { })
            {
                AccentColor = new SolidColorBrush((Color)accentColorResource);
                AccentColor.Freeze();
            }

            Text = text;
            Type = type;
        }

        public ConfirmationButtonInfo(ConfirmationResult type, string text, Color textColor, Color accentColor)
        {
            AccentColor = new SolidColorBrush(accentColor);
            AccentColor.Freeze();
            TextColor = new SolidColorBrush(textColor);
            TextColor.Freeze();
            Text = text;
            Type = type;
        }

        private Brush _accentColor;
        public Brush AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; NotifyOfPropertyChange(); }
        }

        private Brush _textColor;
        public Brush TextColor
        {
            get => _textColor;
            set { _textColor = value; NotifyOfPropertyChange(); }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set { _text = value; NotifyOfPropertyChange(); }
        }

        private ConfirmationResult _type;
        public ConfirmationResult Type
        {
            get => _type;
            set { _type = value; NotifyOfPropertyChange(); }
        }

        public static readonly ConfirmationButtonInfo YesButton = new ConfirmationButtonInfo(ConfirmationResult.Affirmative, "Yes");
        public static readonly ConfirmationButtonInfo NoButton = new ConfirmationButtonInfo(ConfirmationResult.Negative, "No");
        public static readonly ConfirmationButtonInfo CancelButton = new ConfirmationButtonInfo(ConfirmationResult.Cancel, "Cancel");
        public static readonly ConfirmationButtonInfo DeleteButton = new ConfirmationButtonInfo(ConfirmationResult.Affirmative, "Delete", Colors.Red, Colors.Red);

        public static readonly IEnumerable<ConfirmationButtonInfo> YesNo = new[] { YesButton, NoButton };
        public static readonly IEnumerable<ConfirmationButtonInfo> YesNoCancel = new[] { YesButton, NoButton, CancelButton };
        public static readonly IEnumerable<ConfirmationButtonInfo> NoDelete = new[] { NoButton, DeleteButton };
    }
}
