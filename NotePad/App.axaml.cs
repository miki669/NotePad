﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using NotePad.ViewModels;
using NotePad.Views;

namespace NotePad;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    //public override void OnFrameworkInitializationCompleted()
    //{
    //    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    //    {
    //        desktop.MainWindow = new MainWindow
    //        {
    //            DataContext = new MainViewModel()
    //        };
    //    }
    //    else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
    //    {
    //        singleViewPlatform.MainView = new MainView
    //        {
    //            DataContext = new MainViewModel()
    //        };
    //    }

    //    base.OnFrameworkInitializationCompleted();
    //}
    public override void OnFrameworkInitializationCompleted()
    {

        if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new Window
            {
                Content = new LoginForm(),
                DataContext = new LoginFormViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
