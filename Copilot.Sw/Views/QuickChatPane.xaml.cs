using Copilot.Sw.ViewModels;
using System.Windows;
using System.Windows.Interop;

namespace Copilot.Sw.Views;

/// <summary>
/// QuickChatPane.xaml 的交互逻辑
/// </summary>
public partial class QuickChatPane : Window
{
    private readonly IAddin _addin;

    public QuickChatPane(
        IAddin addin,
        QuickChatPaneViewModel quickChatPaneViewModel)
    {
        _addin = addin;

        InitializeComponent();
        var windowInteropHelper = new WindowInteropHelper(this);
        windowInteropHelper.Owner = _addin.SwHandle;

        quickChatPaneViewModel.CloseAction = () => Close();
        DataContext = quickChatPaneViewModel;

        this.Loaded += QuickChatPane_Loaded;

        try
        {
            quickChatPaneViewModel.Init();
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void QuickChatPane_Loaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= QuickChatPane_Loaded;

        var workArea = SystemParameters.WorkArea;

        Left = workArea.Left + workArea.Width * 0.5d - ActualWidth * 0.5d;
        Top = workArea.Top + workArea.Height * 0.8d - ActualHeight * 0.5d;
    }
}
