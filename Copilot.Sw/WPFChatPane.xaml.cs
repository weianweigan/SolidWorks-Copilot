using CommunityToolkit.Mvvm.DependencyInjection;
using Copilot.Sw.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Xarial.XCad.Base.Attributes;

namespace Copilot.Sw;

/// <summary>
/// WPFChatPane.xaml 的交互逻辑
/// </summary>
[Title(AddIn.AddinName)]
[Icon(typeof(Properties.Resources),nameof(Properties.Resources.SolidWorksCopilot))]
public partial class WPFChatPane : UserControl
{
    private WPFChatPaneViewModel _vm;

    public WPFChatPane()
    {
        InitializeComponent();
        DataContext = _vm = Ioc.Default.GetService<WPFChatPaneViewModel>();

        try
        {
            _vm.Init();
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
