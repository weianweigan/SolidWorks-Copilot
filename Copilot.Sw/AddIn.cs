using CommunityToolkit.Mvvm.DependencyInjection;
using Copilot.Sw;
using Copilot.Sw.Config;
using Copilot.Sw.Skills;
using Copilot.Sw.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.UI.Commands;


[ComVisible(true)]
[Guid("9F9212BF-6856-4078-AE4E-F5CD5774DD71")]
[Title(AddinName)]
[Icon(typeof(Copilot.Sw.Properties.Resources), nameof(Copilot.Sw.Properties.Resources.SolidWorksCopilot))]
public class AddIn : SwAddInEx,IAddin
{
    public const string AddinName = "SolidWorks Copilot";

    #region Properties
    ///<inheritdoc/>
    public string AddinDirectory { get; private set; }

    ///<inheritdoc/>
    public IServiceProvider Services { get; private set; }

    public ISldWorks Sw => Application.Sw;
    #endregion

    #region Public Methods
    public override void OnConnect()
    {
        AddinDirectory = Path.GetDirectoryName(typeof(AddIn).Assembly.Location);
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        Services = ConfigureCopilotServices();
        Ioc.Default.ConfigureServices(ConfigureCopilotServices());
        
        var pane = CreateTaskPane<WPFChatPane>();

        CommandManager.AddContextMenu<ContextCommands>(SelectType_e.Everything).CommandClick += AddIn_CommandClick; ;
    }

    public override void OnDisconnect()
    {
        AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
    }
    #endregion

    #region Private Methods
    private void AddIn_CommandClick(ContextCommands spec)
    {

    }

    private IServiceProvider ConfigureCopilotServices()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddSingleton<IAddin>(this);
        
        services.AddSingleton<ITextCompletionProvider,TextCompletionProvider>();
        services.AddSingleton<ISkillsProvider,SkillsProvider>();

        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<WPFChatPaneViewModel>();
        
        services.AddTransient<SettingsWindowViewModel>();

        return services.BuildServiceProvider();
    }

    private Assembly? CurrentDomain_AssemblyResolve(
        object? sender,
        ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name).Name + ".dll";

        try
        {
            var file = Path.Combine(AddinDirectory, assemblyName);

            if (File.Exists(file))
            {
                return Assembly.LoadFrom(file);
            }
            else
            {
                Debug.Print(file);
                return null;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(
                string.Format("The location of the assembly, {0} could not be resolved for loading.", 
                AddinDirectory), ex);
        }
    }
    #endregion
}