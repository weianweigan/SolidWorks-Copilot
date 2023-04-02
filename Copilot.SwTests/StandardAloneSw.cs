using CommunityToolkit.Mvvm.DependencyInjection;
using Copilot.Sw.Config;
using Copilot.Sw.Skills;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolidWorks.Interop.sldworks;
using System.Diagnostics;
using Xarial.XCad.SolidWorks;

namespace Copilot.SwTests;

public class StandandAloneSw : IAddin
{
    private static StandandAloneSw _instance;

    private StandandAloneSw()
    {
        Services = ConfigureCopilotServices();
        Ioc.Default.ConfigureServices(Services);
    }

    internal void LoadSw()
    {
        Sw = StartSw();
    }

    public static StandandAloneSw S_Instance { get => _instance ??= new StandandAloneSw(); }

    public string AddinDirectory => Path.GetDirectoryName(typeof(StandandAloneSw).Assembly.Location);

    public IServiceProvider Services { get; private set; }

    public ISldWorks Sw { get; private set; }

    public IKernel InitKernel()
    {
        IKernel kernel = Kernel.Builder.Build();

        var config = Ioc.Default.GetService<ITextCompletionProvider>()
            .Load()
            .FirstOrDefault();
        if (config == null)
        {
            Assert.Fail("Config your Api key");
        }

        kernel.Config.AddOpenAITextCompletion(
            config.Name,
            config.Model,
            config.Apikey,
            config.Org);

        return kernel;
    }

    private IServiceProvider ConfigureCopilotServices()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddSingleton<IAddin>(this);

        services.AddSingleton<ITextCompletionProvider, TextCompletionProvider>();
        services.AddSingleton<ISkillsProvider, SkillsProvider>();

        return services.BuildServiceProvider();
    }

    private ISldWorks StartSw()
    {
        var swProcess = Process.GetProcessesByName("SLDWORKS");

        if (!swProcess.Any())
        {
            var swApp = SwApplicationFactory
                .Create();
            return swApp.Sw;
        }
        else
        {
            var swApp = SwApplicationFactory.FromProcess(swProcess.First());
            return swApp.Sw;
        }
    }    
}
