using System;
using System.Windows;
using Hospital.App.ViewModels;

namespace Hospital.App.Views;

public partial class LoginWindow : Window
{
    public LoginWindow(LoginViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }

    public LoginWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel)
        {
            viewModel.Password = PasswordBox.Password;
        }
    }

    private void OnDataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is LoginViewModel oldViewModel)
        {
            oldViewModel.LoginSucceeded -= OnLoginSucceeded;
            oldViewModel.RequestClose -= OnRequestClose;
        }

        if (e.NewValue is LoginViewModel newViewModel)
        {
            newViewModel.LoginSucceeded += OnLoginSucceeded;
            newViewModel.RequestClose += OnRequestClose;
        }
    }

    private void OnLoginSucceeded(object? sender, EventArgs e)
    {
        DialogResult = true;
    }

    private void OnRequestClose(object? sender, EventArgs e)
    {
        DialogResult = false;
    }
}
