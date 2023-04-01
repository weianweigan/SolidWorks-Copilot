using CommunityToolkit.Mvvm.DependencyInjection;
using Copilot.Sw.ViewModels;
using System;
using System.Linq;
using Wpf.Ui.Controls.Navigation;

namespace Copilot.Sw.Views;

/// <summary>
/// SettingsWindow.xaml 的交互逻辑
/// </summary>
public partial class SettingsWindow
{
    public SettingsWindow()
    {
        InitializeComponent();  
    }

    public void Save()
    {
        (DataContext as SettingsWindowViewModel)?.Save();
    }
}
