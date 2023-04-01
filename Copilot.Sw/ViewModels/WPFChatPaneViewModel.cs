using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Copilot.Sw.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Configuration;
using System.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.SemanticKernel.KernelExtensions;
using System.IO;
using System.Linq;
using Copilot.Sw.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Copilot.Sw.Config;
using MvvmDialogs;
using Microsoft.SemanticKernel.Memory;
using Copilot.Sw.Skills.SketchSkill;
using Copilot.Sw.Skills;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration.Extensions;

namespace Copilot.Sw.ViewModels;

public partial class WPFChatPaneViewModel : ObservableObject
{
    #region Fields
    private AsyncRelayCommand? _sendCommand;
    private string? _question;
    private readonly ILogger? _logger;
    private readonly IAddin _addin;
    private readonly ITextCompletionProvider _textCompletionProvider;
    private readonly IDialogService _dialogService;
    private bool _configLoadResult;
    #endregion

    #region Ctor
    public WPFChatPaneViewModel(
        IAddin addin,
        ITextCompletionProvider textCompletionProvider,
        IDialogService dialogService)
    {
        _addin = addin;
        _textCompletionProvider = textCompletionProvider;
        _dialogService = dialogService;
        //_logger = logger;
    }
    #endregion

    #region Properties
    public string? Question
    {
        get => _question; set
        {
            SetProperty(ref _question, value);
            SendCommand.NotifyCanExecuteChanged();
        }
    }

    public Conversation Conversation { get; set; } = new();

    public IKernel? Kernel { get; private set; }

    public bool HasItem => Conversation?.Messages?.Any() == true;

    public AsyncRelayCommand SendCommand { get => _sendCommand ??= new AsyncRelayCommand(SendAsync, CanSend); }
    #endregion

    #region Public Methods
    public void Init()
    {
        Kernel = Microsoft.SemanticKernel.Kernel.Builder
            //.WithMemoryStorage(new VolatileMemoryStore())
            //.WithLogger(_logger)
            .Build();
        _configLoadResult = LoadConfigs();
    }

    private bool LoadConfigs()
    {
        var configs = _textCompletionProvider.Load();

        if (configs?.Any() != true)
        {
            return false;
        }

        Kernel.Config.RemoveAllTextCompletionServices();
        foreach (var config in configs)
        {
            if (config.Type == ServerType.OpenAI)
            {
                Kernel.Config.AddOpenAITextCompletion(
                    config.Name,                       // alias used in the prompt templates' config.json
                    config.Model,                     // OpenAI Model Name
                    config.Apikey,            // OpenAI API key
                    config.Org
                    );
            }
            else if (config.Type == ServerType.Azure)
            {
                Kernel.Config.AddAzureOpenAITextCompletion(
                    config.Name,
                    config.Model,
                    config.Apikey,
                    config.Org
                    );
            }
        }
        Kernel.Config.SetDefaultTextCompletionService(configs.First().Name);

        return true;
    }
    #endregion

    #region Private Methods
    [RelayCommand]
    private void Clear()
    {
        Question = "";
    }

    [RelayCommand]
    private void OpenSettings()
    {
        try
        {
            var viewModel = _addin.Services.GetService<SettingsWindowViewModel>();
            if (viewModel == null)
            {
                throw new NullReferenceException();
            }

            var settingWindow = new SettingsWindow() { DataContext = viewModel };
            if (settingWindow.ShowDialog() == true)
            {
                settingWindow.Save();
            }

            _configLoadResult = LoadConfigs();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private bool CanSend() => !string.IsNullOrEmpty(Question);

    private async Task SendAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_question))
        {
            return;
        }

        try
        {
            //check config
            if (!_configLoadResult)
            {
                OpenSettings();
            }

            //copy question
            var question = _question;

            //clear
            Question = "";

            //record question
            Conversation.AddAsk(question);

            //hidden shortitem
            OnPropertyChanged(nameof(HasItem));

            ////use skill
            //var skillDir = Path.Combine(_addin.AddinDirectory, "Skills");
            //var skill = Kernel.ImportSemanticSkillFromDirectory(skillDir, "QASkill");

            ////send
            //var result = await Kernel.RunAsync(
            //    input: question,
            //    cancellationToken: cancellationToken,
            //    skill["Question"]
            //    );

            var planner = Kernel.ImportSkill(new PlannerSkill(Kernel));

            var skill = Kernel.ImportSemanticSkillFromDirectory(
                new SkillsProvider().SkillsLocation,
                "SketchSkill");
            Kernel.ImportSkill(new SketchSegmentCreationSkill());

            var result = await Kernel.RunAsync(
                question,
                skill["CreateSketchSegment"],
                planner["CreatePlan"]);

            var executionResults = result;

            int step = 1;
            int maxSteps = 10;
            while (!executionResults.Variables.ToPlan().IsComplete && step < maxSteps)
            {
                var results = await Kernel.RunAsync(executionResults.Variables, planner["ExecutePlan"]);
                if (results.Variables.ToPlan().IsSuccessful)
                {
                    Console.WriteLine($"Step {step} - Execution results:\n");
                    Console.WriteLine(results.Variables.ToPlan().PlanString);

                    if (results.Variables.ToPlan().IsComplete)
                    {
                        Console.WriteLine($"Step {step} - COMPLETE!");
                        Console.WriteLine(results.Variables.ToPlan().Result);
                        break;
                    }
                }
                else
                {
                    Console.WriteLine($"Step {step} - Execution failed:");
                    Console.WriteLine(results.Variables.ToPlan().Result);
                    break;
                }

                executionResults = results;
                step++;
                Console.WriteLine("");
            }
            Console.WriteLine(result.Variables.ToPlan().PlanString);            

            //check error
            if (result.ErrorOccurred)
            {
                Conversation.AddError(result);
            }

            //response
            Conversation.AddAnswer(result.Variables.ToPlan().Goal);
        }
        catch (Exception ex)
        {
            Conversation.AddError(ex);
        }
        finally
        {
            OnPropertyChanged(nameof(HasItem));
        }
    }
    #endregion
}