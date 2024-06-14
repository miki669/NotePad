using Avalonia;
using Avalonia.Controls;
using NotePad.Message;
using NotePad.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace NotePad.Views
{
    public partial class LoginForm : UserControl
    {

        public LoginForm()
        {
            InitializeComponent();

            // �������� �� ������� ����� ��������� DataContext
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            if (DataContext is LoginFormViewModel viewModel)
            {
                viewModel.LoginSuccessful += OnLoginSuccessful;
                viewModel.LoginFailed += OnLoginFailed; 

            }
        }

        private void OnLoginSuccessful()
        {
            // �������� ��������� ����
            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            // ����� ��������� ����
            mainWindow.Show();

            // �������� �������� ����
            (this.VisualRoot as Window)?.Close();
        }
        private async void OnLoginFailed(string message)
        {
            var dialog = new Window
            {
                Width = 300,
                Height = 150
            };
            dialog.Content = new StackPanel
            {
                Children =
                {
                    new TextBlock { Text = message, Margin = new Thickness(10) },
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Command = ReactiveCommand.Create(() => (dialog as Window)?.Close())
                    },
                    new Button
                    {
                        Content = "������������������",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Command = ReactiveCommand.Create(() =>
                        {
                            (dialog as Window)?.Close();
                            OpenRegistrationWindow();
                        })
                    }
                }
            };
            await dialog.ShowDialog((this.VisualRoot as Window));
        }
        private void OpenRegistrationWindow()
        {
            var registrationWindow = new RegistrationWindow
            {
                DataContext = new RegistrationViewModel()
            };

            ((RegistrationViewModel)registrationWindow.DataContext).RegistrationSuccessful += () =>
            {
                registrationWindow.Close();
                // ����� ����� �������� ������ ��� ���������� �������� ���� ��� ���������� ������ �������� ����� �������� �����������
            };

            registrationWindow.ShowDialog((this.VisualRoot as Window));
        }
    }
}
