﻿using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;

namespace WPFCustomMessageBox
{
    /// <summary>
    /// Interaction logic for ModalDialog.xaml
    /// </summary>
    internal partial class CustomMessageBoxWindow : Window
    {
        private readonly bool _removeTitleBarIcon = true;

        public string Caption
        {
            get
            {
                return Title;
            }
            set
            {
                Title = value;
            }
        }

        public string Message
        {
            get
            {
                return TextBlock_Message.Text;
            }
            set
            {
                TextBlock_Message.Text = value;
            }
        }

        public string OkButtonText
        {
            get
            {
                return Label_Ok.Content.ToString();
            }
            set
            {
                Label_Ok.Content = value.TryAddKeyboardAccellerator();
            }
        }

        public string CancelButtonText
        {
            get
            {
                return Label_Cancel.Content.ToString();
            }
            set
            {
                Label_Cancel.Content = value.TryAddKeyboardAccellerator();
            }
        }

        public string YesButtonText
        {
            get
            {
                return Label_Yes.Content.ToString();
            }
            set
            {
                Label_Yes.Content = value.TryAddKeyboardAccellerator();
            }
        }

        public string NoButtonText
        {
            get
            {
                return Label_No.Content.ToString();
            }
            set
            {
                Label_No.Content = value.TryAddKeyboardAccellerator();
            }
        }

        public MessageBoxResult Result { get; set; }

        internal CustomMessageBoxWindow(Window owner, string message, string caption = null, MessageBoxButton? button = null, MessageBoxImage? image = null, bool removeTitleBarIcon = true)
        {
            InitializeComponent();

            _removeTitleBarIcon = removeTitleBarIcon;
            Focusable = true;
            ShowActivated = true;
            ShowInTaskbar = true;

            if (owner != null && owner.IsLoaded)
            {
                Owner = owner;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            Message = message;
            Caption = caption;

            DisplayButtons(button ?? MessageBoxButton.OK);

            if (image.HasValue)
                DisplayImage(image.Value);
            else
                Image_MessageBox.Visibility = Visibility.Collapsed;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            if (_removeTitleBarIcon)
                Util.RemoveIcon(this);

            base.OnSourceInitialized(e);
        }

        private void DisplayButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    // Hide all but OK, Cancel
                    Button_OK.Visibility = Visibility.Visible;
                    Button_OK.Focus();
                    Button_Cancel.Visibility = Visibility.Visible;

                    Button_Yes.Visibility = Visibility.Collapsed;
                    Button_No.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    // Hide all but Yes, No
                    Button_Yes.Visibility = Visibility.Visible;
                    Button_Yes.Focus();
                    Button_No.Visibility = Visibility.Visible;

                    Button_OK.Visibility = Visibility.Collapsed;
                    Button_Cancel.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    // Hide only OK
                    Button_Yes.Visibility = Visibility.Visible;
                    Button_Yes.Focus();
                    Button_No.Visibility = Visibility.Visible;
                    Button_Cancel.Visibility = Visibility.Visible;

                    Button_OK.Visibility = Visibility.Collapsed;
                    break;
                default:
                    // Hide all but OK
                    Button_OK.Visibility = Visibility.Visible;
                    Button_OK.Focus();

                    Button_Yes.Visibility = Visibility.Collapsed;
                    Button_No.Visibility = Visibility.Collapsed;
                    Button_Cancel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void DisplayImage(MessageBoxImage image)
        {
            Icon icon;

            switch (image)
            {
                case MessageBoxImage.Exclamation:       // Enumeration value 48 - also covers "Warning"
                    icon = SystemIcons.Exclamation;
                    break;
                case MessageBoxImage.Error:             // Enumeration value 16, also covers "Hand" and "Stop"
                    icon = SystemIcons.Hand;
                    break;
                case MessageBoxImage.Information:       // Enumeration value 64 - also covers "Asterisk"
                    icon = SystemIcons.Information;
                    break;
                case MessageBoxImage.Question:
                    icon = SystemIcons.Question;
                    break;
                default:
                    icon = SystemIcons.Information;
                    break;
            }

            Image_MessageBox.Source = icon.ToImageSource();
            Image_MessageBox.Visibility = Visibility.Visible;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void Button_Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void Button_No_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        /// <summary>
        /// Handler for Title MouseDown event
        /// </summary>
        private void TitleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
