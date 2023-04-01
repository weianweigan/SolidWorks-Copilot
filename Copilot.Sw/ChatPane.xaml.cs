using System;
using System.IO;
using System.Windows.Controls;
using Xarial.XCad.Base.Attributes;

namespace Copilot.Sw;

/// <summary>
/// ChatPane.xaml 的交互逻辑
/// </summary>
[Title(AddIn.AddinName)]
public partial class ChatPane : UserControl
{
    public ChatPane()
    {
        InitializeComponent();

        webView.CreationProperties = new Microsoft.Web.WebView2.Wpf.CoreWebView2CreationProperties
        {
            UserDataFolder = InitUserDataFolder()
        };
    }

    private static string InitUserDataFolder()
    {
        string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AddIn.AddinName);
        if (!Directory.Exists(userDataFolder))
            Directory.CreateDirectory(userDataFolder);

        return userDataFolder;
    }
}
